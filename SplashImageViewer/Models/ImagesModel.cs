namespace SplashImageViewer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public enum ExifTag : int
    {
        // Exif file format: https://www.media.mit.edu/pia/Research/deepview/exif.html
        ImageDescription = 0x010e, // ascii string - Describes image
        Orientation = 0x0112,      // unsigned short - The orientation of the camera relative to the scene, when the image was captured. The start point of stored data is, '1' means upper left, '3' lower right, '6' upper right, '8' lower left, '9' undefined.
        Software = 0x0131,         // ascii string - Shows firmware(internal software of digicam) version number.
        Copyright = 0x8298,        // ascii string - Shows copyright information.
    }

    public class ImagesModel
    {
        private static readonly Random Rnd = new Random();

        private readonly IReadOnlyList<string> fileExtensions = new string[] { ".jpg", ".jpe", ".jpeg", ".jfif", ".bmp", ".png", ".gif", ".ico" };
        private readonly object locker;
        private readonly object locker2;

        private int index;
        private List<string> filePaths;
        private FileSystemWatcher? fileWatcher;
        private SearchOption searchOption;

        private ImagesModel()
        {
            locker = new object();
            locker2 = new object();
            filePaths = new List<string>();
        }

        public delegate void CustomEventHandler(object sender);

        /// <summary>
        /// Gets ImagesModel singleton instance.
        /// </summary>
        public static ImagesModel Singleton { get; } = new ImagesModel();

        public CustomEventHandler? CurrentFilePathIndexChanged { get; set; }

        public IReadOnlyList<string> FilePaths => filePaths.AsReadOnly();

        public Image? Image { get; private set; }

        public ImageFormat? ImageRawFormat { get; private set; }

        public string? ImageFormatDescription { get; private set; }

        public string CurrentFilePath => (index < filePaths.Count) ? filePaths[index] : string.Empty;

        public int CurrentFilePathIndex
        {
            get => index;
            private set
            {
                index = value;

                lock (locker)
                {
                    // notify event subscribers (if they exist), that current file index has changed
                    CurrentFilePathIndexChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Initializes required fields/properties.
        /// </summary>
        public void Init(string path, SearchOption so)
        {
            // get the file attributes for file or directory
            var fa = File.GetAttributes(path);
            string? dir = fa.HasFlag(FileAttributes.Directory) ? path : Path.GetDirectoryName(path);

            filePaths = Directory.EnumerateFiles(dir, "*.*", so)
                                 .Where(s => fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

            if (filePaths.Count == 0)
            {
                throw new Exception($"'{dir}' directory has no images");
            }

            searchOption = so;

            // create a new FileSystemWatcher and set its properties
            fileWatcher = new FileSystemWatcher
            {
                Path = dir,
                Filter = "*.*",
                IncludeSubdirectories = so == SearchOption.AllDirectories, // watch both files and subdirectories
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName, // watch for all changes specified in the NotifyFilters enumeration
            };

            // add event handlers
            fileWatcher.Created += FileCreatedOrDeletedEvent;
            fileWatcher.Deleted += FileCreatedOrDeletedEvent;
            fileWatcher.Renamed += FileRenamedEvent;

            // start monitoring
            fileWatcher.EnableRaisingEvents = true;

            if (!fa.HasFlag(FileAttributes.Directory))
            {
                int index = filePaths.IndexOf(path);

                if (index == -1)
                {
                    throw new Exception($"Unsupported file type: '{path}'");
                }

                CurrentFilePathIndex = index;
            }
        }

        /// <summary>
        /// Releases used resources.
        /// </summary>
        public void DisposeResources()
        {
            if (Image is object)
            {
                Image.Dispose();
                Image = null;
            }

            if (fileWatcher is object)
            {
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Created -= FileCreatedOrDeletedEvent;
                fileWatcher.Deleted -= FileCreatedOrDeletedEvent;
                fileWatcher.Renamed -= FileRenamedEvent;
                fileWatcher.Dispose();
                fileWatcher = null;
            }
        }

        public void SelectNextImageIndex()
        {
            if (FilePaths.Count > 1)
            {
                CurrentFilePathIndex = (CurrentFilePathIndex == FilePaths.Count - 1) ? 0 : ++CurrentFilePathIndex;
            }
        }

        public void SelectPreviousImageIndex()
        {
            if (FilePaths.Count > 1)
            {
                CurrentFilePathIndex = (CurrentFilePathIndex == 0) ? FilePaths.Count - 1 : --CurrentFilePathIndex;
            }
        }

        public void SelectRandomImageIndex()
        {
            if (FilePaths.Count > 1)
            {
                while (true)
                {
                    int tmp = Rnd.Next(FilePaths.Count);

                    if (CurrentFilePathIndex != tmp)
                    {
                        CurrentFilePathIndex = tmp;
                        break;
                    }
                }
            }
        }

        public void LoadImage()
        {
            Image?.Dispose();

            using var fs = new FileStream(CurrentFilePath, FileMode.Open, FileAccess.Read); // open file in read only mode
            using var br = new BinaryReader(fs); // get a binary reader for the file stream
            var ms = new MemoryStream(br.ReadBytes((int)fs.Length)); // copy the content of the file into a memory stream
            Image = Image.FromStream(ms);
            ImageRawFormat = new ImageFormat(Image.RawFormat.Guid);
            ImageFormatDescription = GetImageFormatDescription(Image.RawFormat);
            ProcessImageMetadata(Image);
        }

        public void OverwriteImage()
        {
            if (Image is null)
            {
                throw new NullReferenceException(nameof(Image));
            }

            ModifyImageMetadata(Image);

            var ici = GetEncoder(ImageRawFormat);

            // Create an Encoder object based on the GUID for the Quality parameter category.
            var encoder = Encoder.Quality;

            // Create an EncoderParameters object. An EncoderParameters object has an array of EncoderParameter objects. In this case, there is only one EncoderParameter object in the array.
            using var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(encoder, 100L);

            Image.Save(CurrentFilePath, ici, encoderParameters);
        }

        public void DeleteImage()
        {
            lock (locker2)
            {
                File.Delete(CurrentFilePath);
            }
        }

        private void FileCreatedOrDeletedEvent(object sender, FileSystemEventArgs e)
        {
            lock (locker2)
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    // wait until file is fully written and released by the os
                    if (CheckFileIsLocked(e.FullPath))
                    {
                        // throw new Exception($"'{e.FullPath}' file is locked");
                        return;
                    }
                }

                // save previous file path variable before creating new _filePaths list
                string previousFilePath = CurrentFilePath;
                int previousIndex = CurrentFilePathIndex;

                filePaths = Directory.EnumerateFiles(Path.GetDirectoryName(e.FullPath), "*.*", searchOption)
                                  .Where(s => fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (filePaths.Count == 0)
                {
                    // invoke CurrentFilePathIndexChanged event on different thread, so that this method is not blocked and (filewatcher dispose)
                    Task.Run(() => { CurrentFilePathIndex = 0; });
                    return;
                }

                int index = filePaths.IndexOf(previousFilePath);

                if (index != -1)
                {
                    CurrentFilePathIndex = index;
                }
                else if (filePaths.Count > previousIndex)
                {
                    CurrentFilePathIndex = previousIndex;
                }
                else
                {
                    --CurrentFilePathIndex;
                }
            }
        }

        private void FileRenamedEvent(object sender, RenamedEventArgs e)
        {
            lock (locker2)
            {
                if (e.ChangeType == WatcherChangeTypes.Renamed)
                {
                    // wait until file is fully written and released by the os
                    if (CheckFileIsLocked(e.FullPath))
                    {
                        // throw new Exception($"'{e.FullPath}' file is locked");
                        return;
                    }
                }

                // save previous file path variable before creating new _filePaths
                string previousFilePath = CurrentFilePath;
                int previousIndex = CurrentFilePathIndex;

                filePaths = Directory.EnumerateFiles(Path.GetDirectoryName(e.FullPath), "*.*", searchOption)
                                  .Where(s => fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (filePaths.Count == 0)
                {
                    // invoke CurrentFilePathIndexChanged event on different thread, so that this method is not blocked and (filewatcher dispose)
                    Task.Run(() => { CurrentFilePathIndex = 0; });
                    return;
                }

                int index = filePaths.IndexOf(previousFilePath);

                if (index != -1)
                {
                    CurrentFilePathIndex = index;
                }
                else if (filePaths.Count > previousIndex)
                {
                    CurrentFilePathIndex = previousIndex;
                }
                else
                {
                    --CurrentFilePathIndex;
                }
            }
        }

        private bool CheckFileIsLocked(string path)
        {
            int tries = 0;

            while (tries < 5)
            {
                ++tries;

                try
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    return false;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }

            return true;
        }

        private void ProcessImageMetadata(Image img)
        {
            // var items = Image.PropertyItems;
            if (img.PropertyIdList.Contains((int)ExifTag.Orientation))
            {
                var item = img.GetPropertyItem((int)ExifTag.Orientation);

                if (item.Value[0] != 1)
                {
                    if (item.Value[0] == 3)
                    {
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (item.Value[0] == 6)
                    {
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (item.Value[0] == 8)
                    {
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                }
            }
        }

        private void ModifyImageMetadata(Image img)
        {
            if (img.PropertyIdList.Contains((int)ExifTag.Software))
            {
                var item = img.GetPropertyItem((int)ExifTag.Software);

                // set image software version tag
                var bytes = System.Text.Encoding.ASCII.GetBytes(ApplicationInfo.AppHeader + '\0');
                item.Len = bytes.Length;
                item.Value = bytes;
                img.SetPropertyItem(item);
            }

            if (img.PropertyIdList.Contains((int)ExifTag.Orientation))
            {
                var item = img.GetPropertyItem((int)ExifTag.Orientation);

                // set image orientation tag
                item.Value[0] = 1;
                img.SetPropertyItem(item);

                // img.RemovePropertyItem((int)ExifTag.Orientation);
            }
        }

        private ImageCodecInfo? GetEncoder(ImageFormat? format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format?.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        private string GetImageFormatDescription(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec.FormatDescription;
                }
            }

            return "UNKNOWN";
        }
    }
}
