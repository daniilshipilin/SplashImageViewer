using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SplashImageViewer.Models
{
    public class ImagesModel : IDisposable
    {
        // Exif file format: https://www.media.mit.edu/pia/Research/deepview/exif.html
        // Check image rotation flags:
        // 0x010e - ImageDescription - ascii string - Describes image
        // 0x0112 - Orientation - unsigned short - The orientation of the camera relative to the scene, when the image was captured. The start point of stored data is, '1' means upper left, '3' lower right, '6' upper right, '8' lower left, '9' undefined.
        // 0x0131 - Software - ascii string - Shows firmware(internal software of digicam) version number.
        // 0x8298 - Copyright - ascii string - Shows copyright information.

        public enum ExifTag : int
        {
            ImageDescription = 0x010e,
            Orientation = 0x0112,
            Software = 0x0131,
            Copyright = 0x8298
        }

        public delegate void CustomEventHandler(object sender);
        public CustomEventHandler CurrentFilePathIndexChanged;

        public IReadOnlyList<string> FilePaths => _filePaths.AsReadOnly();
        public Image Image { get; private set; }
        public ImageFormat ImageRawFormat { get; private set; }
        public string ImageFormatDescription { get; private set; }
        public string CurrentFilePath => (CurrentFilePathIndex < _filePaths.Count) ? _filePaths[CurrentFilePathIndex] : null;
        public int CurrentFilePathIndex
        {
            get => _index;
            private set
            {
                _index = value;

                lock (_locker)
                {
                    // notify event subscribers (if they exist), that current file index has changed
                    CurrentFilePathIndexChanged?.Invoke(this);
                }
            }
        }

        int _index;
        List<string> _filePaths;
        readonly string[] _fileExtensions = new string[] { ".jpg", ".jpe", ".jpeg", ".jfif", ".bmp", ".png", ".gif", ".ico" };
        static readonly Random _rnd = new Random();
        readonly object _locker = new object();
        readonly object _locker2 = new object();
        readonly FileSystemWatcher _fileWatcher;
        readonly SearchOption _so;

        public ImagesModel(string dir, SearchOption so)
        {
            _filePaths = Directory.EnumerateFiles(dir, "*.*", so)
                                 .Where(s => _fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

            if (_filePaths.Count == 0)
            {
                throw new Exception($"'{dir}' directory has no images");
            }

            _so = so;

            // create a new FileSystemWatcher and set its properties
            _fileWatcher = new FileSystemWatcher
            {
                Path = dir,
                Filter = "*.*",
                // watch both files and subdirectories
                IncludeSubdirectories = (so == SearchOption.AllDirectories),
                // watch for all changes specified in the NotifyFilters enumeration
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            // add event handlers
            _fileWatcher.Created += FileCreatedOrDeletedEvent;
            _fileWatcher.Deleted += FileCreatedOrDeletedEvent;
            _fileWatcher.Renamed += FileRenamedEvent;

            // start monitoring
            _fileWatcher.EnableRaisingEvents = true;
        }

        public ImagesModel(string dir, string filePath, SearchOption so) : this(dir, so)
        {
            int index = _filePaths.IndexOf(filePath);

            if (index == -1)
            {
                throw new Exception($"Cannot find '{filePath}'");
            }

            CurrentFilePathIndex = index;
        }

        private void FileCreatedOrDeletedEvent(object sender, FileSystemEventArgs e)
        {
            lock (_locker2)
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    // wait until file is fully written and released by the os
                    if (CheckFileIsLocked(e.FullPath))
                    {
                        //throw new Exception($"'{e.FullPath}' file is locked");
                        return;
                    }
                }

                // save previous file path variable before creating new _filePaths list
                string previousFilePath = CurrentFilePath;
                int previousIndex = CurrentFilePathIndex;

                _filePaths = Directory.EnumerateFiles(Path.GetDirectoryName(e.FullPath), "*.*", _so)
                                  .Where(s => _fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (_filePaths.Count == 0)
                {
                    // invoke CurrentFilePathIndexChanged event on different thread, so that this method is not blocked and (filewatcher dispose)
                    Task.Run(() => { CurrentFilePathIndex = 0; });
                    return;
                }

                int index = _filePaths.IndexOf(previousFilePath);

                if (index != -1)
                {
                    CurrentFilePathIndex = index;
                }
                else if (_filePaths.Count > previousIndex)
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
            lock (_locker2)
            {
                if (e.ChangeType == WatcherChangeTypes.Renamed)
                {
                    // wait until file is fully written and released by the os
                    if (CheckFileIsLocked(e.FullPath))
                    {
                        //throw new Exception($"'{e.FullPath}' file is locked");
                        return;
                    }
                }

                // save previous file path variable before creating new _filePaths
                string previousFilePath = CurrentFilePath;
                int previousIndex = CurrentFilePathIndex;

                _filePaths = Directory.EnumerateFiles(Path.GetDirectoryName(e.FullPath), "*.*", _so)
                                  .Where(s => _fileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (_filePaths.Count == 0)
                {
                    // invoke CurrentFilePathIndexChanged event on different thread, so that this method is not blocked and (filewatcher dispose)
                    Task.Run(() => { CurrentFilePathIndex = 0; });
                    return;
                }

                int index = _filePaths.IndexOf(previousFilePath);

                if (index != -1)
                {
                    CurrentFilePathIndex = index;
                }
                else if (_filePaths.Count > previousIndex)
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

        public void SelectNextImageIndex()
        {
            if (_filePaths.Count <= 1) { return; }

            CurrentFilePathIndex = (CurrentFilePathIndex == _filePaths.Count - 1) ? 0 : ++CurrentFilePathIndex;
        }

        public void SelectPreviousImageIndex()
        {
            if (_filePaths.Count <= 1) { return; }

            CurrentFilePathIndex = (CurrentFilePathIndex == 0) ? _filePaths.Count - 1 : --CurrentFilePathIndex;
        }

        public void SelectRandomImageIndex()
        {
            if (_filePaths.Count <= 1) { return; }

            while (true)
            {
                int tmp = _rnd.Next(0, _filePaths.Count);

                if (CurrentFilePathIndex != tmp)
                {
                    CurrentFilePathIndex = tmp;
                    break;
                }
            }
        }

        public void LoadImage()
        {
            Image?.Dispose();

            //open file in read only mode
            using var fs = new FileStream(CurrentFilePath, FileMode.Open, FileAccess.Read);
            // get a binary reader for the file stream
            using var br = new BinaryReader(fs);
            // copy the content of the file into a memory stream
            var ms = new MemoryStream(br.ReadBytes((int)fs.Length));
            Image = Image.FromStream(ms);
            //Image = new Bitmap(ms);

            //Image = Image.FromFile(CurrentFilePath);
            ImageRawFormat = new ImageFormat(Image.RawFormat.Guid);
            ImageFormatDescription = GetImageFormatDescription(Image.RawFormat);
            ProcessImageMetadata();
        }

        private void ProcessImageMetadata()
        {
            //var items = Image.PropertyItems;
            if (Image.PropertyIdList.Contains((int)ExifTag.Orientation))
            {
                var item = Image.GetPropertyItem((int)ExifTag.Orientation);

                if (item.Value[0] != 1)
                {
                    if (item.Value[0] == 3) { Image.RotateFlip(RotateFlipType.Rotate180FlipNone); }
                    else if (item.Value[0] == 6) { Image.RotateFlip(RotateFlipType.Rotate90FlipNone); }
                    else if (item.Value[0] == 8) { Image.RotateFlip(RotateFlipType.Rotate270FlipNone); }
                }
            }
        }

        private void ModifyImageMetadata()
        {
            if (Image.PropertyIdList.Contains((int)ExifTag.Software))
            {
                var item = Image.GetPropertyItem((int)ExifTag.Software);

                // set image software version tag
                var bytes = System.Text.Encoding.ASCII.GetBytes(ApplicationInfo.AppHeader + '\0');
                item.Len = bytes.Length;
                item.Value = bytes;
                Image.SetPropertyItem(item);
            }

            if (Image.PropertyIdList.Contains((int)ExifTag.Orientation))
            {
                var item = Image.GetPropertyItem((int)ExifTag.Orientation);

                // set image orientation tag
                item.Value[0] = 1;
                Image.SetPropertyItem(item);
                //Image.RemovePropertyItem((int)ExifTag.Orientation);
            }
        }

        public void OverwriteImage()
        {
            ModifyImageMetadata();

            var ici = GetEncoder(ImageRawFormat);

            // Create an Encoder object based on the GUID for the Quality parameter category.
            var encoder = Encoder.Quality;

            // Create an EncoderParameters object. An EncoderParameters object has an array of EncoderParameter objects. In this case, there is only one EncoderParameter object in the array.
            using var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(encoder, (long)100);
            Image.Save(CurrentFilePath, ici, encoderParameters);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
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

        public void DeleteImage()
        {
            lock (_locker2)
            {
                File.Delete(CurrentFilePath);
            }
        }

        public void Dispose()
        {
            Image?.Dispose();
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Created -= FileCreatedOrDeletedEvent;
            _fileWatcher.Deleted -= FileCreatedOrDeletedEvent;
            _fileWatcher.Renamed -= FileRenamedEvent;
            _fileWatcher?.Dispose();
        }
    }
}
