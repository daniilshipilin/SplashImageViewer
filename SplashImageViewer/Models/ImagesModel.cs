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

    /// <summary>
    /// Exif file format enum.<br/>
    /// Link: <see href="https://www.media.mit.edu/pia/Research/deepview/exif.html"/>
    /// </summary>
    public enum ExifTag : int
    {
        ImageDescription = 0x010e, // ascii string - Describes image
        Orientation = 0x0112,      // unsigned short - The orientation of the camera relative to the scene, when the image was captured. The start point of stored data is, '1' means upper left, '3' lower right, '6' upper right, '8' lower left, '9' undefined.
        Software = 0x0131,         // ascii string - Shows firmware(internal software of digicam) version number.
        Copyright = 0x8298,        // ascii string - Shows copyright information.
    }

    public class ImagesModel
    {
        private static readonly Random Rnd = new();

        private readonly IReadOnlyList<string> fileExtensions = new string[] { ".jpg", ".jpe", ".jpeg", ".jfif", ".bmp", ".png", ".gif", ".ico" };
        private readonly object locker;
        private readonly object locker2;

        private int index;
        private List<string> filePaths;
        private FileSystemWatcher? fileWatcher;
        private SearchOption searchOption;

        private ImagesModel()
        {
            this.locker = new object();
            this.locker2 = new object();
            this.filePaths = new List<string>();
        }

        public delegate void CustomEventHandler(object sender);

        /// <summary>
        /// Gets ImagesModel singleton instance.
        /// </summary>
        public static ImagesModel Singleton { get; } = new ImagesModel();

        public CustomEventHandler? CurrentFilePathIndexChanged { get; set; }

        public IReadOnlyList<string> FilePaths => this.filePaths.AsReadOnly();

        public Image? Image { get; private set; }

        public ImageFormat? ImageRawFormat { get; private set; }

        public string ImageFormatDescription { get; private set; } = string.Empty;

        public string CurrentFilePath => (this.index < this.filePaths.Count) ? this.filePaths[this.index] : string.Empty;

        public string CurrentFileName => (this.index < this.filePaths.Count) ? Path.GetFileName(this.filePaths[this.index]) : string.Empty;

        public int CurrentFilePathIndex
        {
            get => this.index;

            private set
            {
                this.index = value;

                lock (this.locker)
                {
                    // notify event subscribers (if they exist), that current file index has changed
                    this.CurrentFilePathIndexChanged?.Invoke(this);
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
            string dir = fa.HasFlag(FileAttributes.Directory) ? path : Path.GetDirectoryName(path) ?? string.Empty;

            this.filePaths = Directory.EnumerateFiles(dir, "*.*", so)
                                 .Where(s => this.fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

            if (this.filePaths.Count == 0)
            {
                throw new Exception($"'{dir}' directory has no images");
            }

            this.searchOption = so;

            // create a new FileSystemWatcher and set its properties
            this.fileWatcher = new FileSystemWatcher
            {
                Path = dir,
                Filter = "*.*",
                IncludeSubdirectories = so == SearchOption.AllDirectories, // watch both files and subdirectories
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName, // watch for all changes specified in the NotifyFilters enumeration
            };

            // add event handlers
            this.fileWatcher.Created += this.FileCreatedOrDeletedEvent;
            this.fileWatcher.Deleted += this.FileCreatedOrDeletedEvent;
            this.fileWatcher.Renamed += this.FileRenamedEvent;

            // start monitoring
            this.fileWatcher.EnableRaisingEvents = true;

            this.CurrentFilePathIndex = 0;

            if (!fa.HasFlag(FileAttributes.Directory))
            {
                int index = this.filePaths.IndexOf(path);

                if (index == -1)
                {
                    throw new Exception($"Unsupported file type: '{path}'");
                }

                this.CurrentFilePathIndex = index;
            }
        }

        /// <summary>
        /// Releases used resources.
        /// </summary>
        public void DisposeResources()
        {
            if (this.Image is not null)
            {
                this.Image.Dispose();
                this.Image = null;
            }

            if (this.fileWatcher is not null)
            {
                this.fileWatcher.EnableRaisingEvents = false;
                this.fileWatcher.Created -= this.FileCreatedOrDeletedEvent;
                this.fileWatcher.Deleted -= this.FileCreatedOrDeletedEvent;
                this.fileWatcher.Renamed -= this.FileRenamedEvent;
                this.fileWatcher.Dispose();
                this.fileWatcher = null;
            }
        }

        public void SelectNextImageIndex()
        {
            if (this.FilePaths.Count > 1)
            {
                this.CurrentFilePathIndex = (this.CurrentFilePathIndex == this.FilePaths.Count - 1) ? 0 : this.CurrentFilePathIndex + 1;
            }
        }

        public void SelectPreviousImageIndex()
        {
            if (this.FilePaths.Count > 1)
            {
                this.CurrentFilePathIndex = (this.CurrentFilePathIndex == 0) ? this.FilePaths.Count - 1 : this.CurrentFilePathIndex - 1;
            }
        }

        public void SelectRandomImageIndex()
        {
            if (this.FilePaths.Count > 1)
            {
                while (true)
                {
                    int tmp = Rnd.Next(this.FilePaths.Count);

                    if (this.CurrentFilePathIndex != tmp)
                    {
                        this.CurrentFilePathIndex = tmp;
                        break;
                    }
                }
            }
        }

        public void LoadImage()
        {
            this.Image?.Dispose();

            using var fs = new FileStream(this.CurrentFilePath, FileMode.Open, FileAccess.Read); // open file in read only mode
            using var br = new BinaryReader(fs); // get a binary reader for the file stream
            var ms = new MemoryStream(br.ReadBytes((int)fs.Length)); // copy the content of the file into a memory stream
            this.Image = Image.FromStream(ms);
            this.ImageRawFormat = new ImageFormat(this.Image.RawFormat.Guid);
            this.ImageFormatDescription = GetImageFormatDescription(this.Image.RawFormat);
            ProcessImageMetadata(this.Image);
        }

        public void OverwriteImage()
        {
            if (this.Image is null)
            {
                throw new NullReferenceException(nameof(this.Image));
            }

            ModifyImageMetadata(this.Image);

            // Create an Encoder object based on the GUID for the Quality parameter category.
            var encoder = Encoder.Quality;

            // Create an EncoderParameters object. An EncoderParameters object has an array of EncoderParameter objects. In this case, there is only one EncoderParameter object in the array.
            using var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(encoder, 100L);

            var ici = GetEncoder(this.ImageRawFormat);

            if (ici is null)
            {
                throw new NullReferenceException(nameof(ici));
            }

            this.Image.Save(this.CurrentFilePath, ici, encoderParameters);
        }

        public void DeleteImage()
        {
            lock (this.locker2)
            {
                File.Delete(this.CurrentFilePath);
            }
        }

        private static bool CheckFileIsLocked(string path)
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

        private static void ProcessImageMetadata(Image img)
        {
            // var items = Image.PropertyItems;
            if (img.PropertyIdList.Contains((int)ExifTag.Orientation))
            {
                var item = img.GetPropertyItem((int)ExifTag.Orientation);

                if (item is not null && item.Value is not null)
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

        private static void ModifyImageMetadata(Image img)
        {
            if (img.PropertyIdList.Contains((int)ExifTag.Software))
            {
                var item = img.GetPropertyItem((int)ExifTag.Software);

                if (item is not null)
                {
                    // set image software version tag
                    byte[]? bytes = System.Text.Encoding.ASCII.GetBytes(ApplicationInfo.AppHeader + '\0');
                    item.Len = bytes.Length;
                    item.Value = bytes;
                    img.SetPropertyItem(item);
                }
            }

            if (img.PropertyIdList.Contains((int)ExifTag.Orientation))
            {
                var item = img.GetPropertyItem((int)ExifTag.Orientation);

                if (item is not null && item.Value is not null)
                {
                    // set image orientation tag
                    item.Value[0] = 1;
                    img.SetPropertyItem(item);

                    // img.RemovePropertyItem((int)ExifTag.Orientation);
                }
            }
        }

        private static ImageCodecInfo? GetEncoder(ImageFormat? format)
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

        private static string GetImageFormatDescription(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec.FormatDescription ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private void FileCreatedOrDeletedEvent(object sender, FileSystemEventArgs e)
        {
            lock (this.locker2)
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
                string previousFilePath = this.CurrentFilePath;
                int previousIndex = this.CurrentFilePathIndex;

                string? dir = Path.GetDirectoryName(e.FullPath);

                if (dir is null)
                {
                    throw new NullReferenceException(nameof(dir));
                }

                this.filePaths = Directory.EnumerateFiles(dir, "*.*", this.searchOption)
                                  .Where(s => this.fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (this.filePaths.Count == 0)
                {
                    // invoke CurrentFilePathIndexChanged event on different thread, so that this method is not blocked and (filewatcher dispose)
                    Task.Run(() => { this.CurrentFilePathIndex = 0; });
                    return;
                }

                int index = this.filePaths.IndexOf(previousFilePath);

                if (index != -1)
                {
                    this.CurrentFilePathIndex = index;
                }
                else if (this.filePaths.Count > previousIndex)
                {
                    this.CurrentFilePathIndex = previousIndex;
                }
                else
                {
                    --this.CurrentFilePathIndex;
                }
            }
        }

        private void FileRenamedEvent(object sender, RenamedEventArgs e)
        {
            lock (this.locker2)
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
                string previousFilePath = this.CurrentFilePath;
                int previousIndex = this.CurrentFilePathIndex;

                string? dir = Path.GetDirectoryName(e.FullPath);

                if (dir is null)
                {
                    throw new NullReferenceException(nameof(dir));
                }

                this.filePaths = Directory.EnumerateFiles(dir, "*.*", this.searchOption)
                                  .Where(s => this.fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (this.filePaths.Count == 0)
                {
                    // invoke CurrentFilePathIndexChanged event on different thread, so that this method is not blocked and (filewatcher dispose)
                    Task.Run(() => { this.CurrentFilePathIndex = 0; });
                    return;
                }

                int index = this.filePaths.IndexOf(previousFilePath);

                if (index != -1)
                {
                    this.CurrentFilePathIndex = index;
                }
                else if (this.filePaths.Count > previousIndex)
                {
                    this.CurrentFilePathIndex = previousIndex;
                }
                else
                {
                    --this.CurrentFilePathIndex;
                }
            }
        }
    }
}
