using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Audio;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using MomoViewer.Controls;
using MomoViewer.Model;

namespace MomoViewer.ViewModel
{
    public class ChapterVM : ViewModelBase
    {
        public ChapterVM()
        {
            _chapter = new ChapterInfo();
        }
        private ChapterInfo _chapter;

        public ChapterInfo Chapter
        {
            get { return _chapter; }
            set
            {
                _chapter = value;
                RaisePropertyChanged();
            }
        }

        public async Task<ChapterInfo> LoadChapterFromFolder()
        {
            var picker = new FolderPicker();
            Chapter.Pages = new List<PageInfo>();
            picker.FileTypeFilter.Add("*");
            var fd = await picker.PickSingleFolderAsync();
            if (fd != null)
            {
                var fileList = await fd.GetFilesAsync();
                int pageNumber = 0;
                foreach (var storageFile in fileList)
                {
                    if (storageFile.IsOfType(StorageItemTypes.File))
                    {
                        string mediaType = storageFile.ContentType.Split('/')[0];
                        if (mediaType == "image")
                        {
                            PageInfo page = new PageInfo();
                            page.Path = storageFile.Path;
                            page.Number = pageNumber++;
                            using (var stream = await storageFile.OpenAsync(FileAccessMode.Read))
                            {
                                var bitmapImage = new BitmapImage();
                                await bitmapImage.SetSourceAsync(stream);
                                page.Image = bitmapImage;
                            }
                            Chapter.Pages.Add(page);
                        }
                    }
                }
            }
            return Chapter;
        }

        public async Task<ChapterInfo> LoadChapterFromFile()
        {
            var picker = new FileOpenPicker();
            Chapter.Pages = new List<PageInfo>();
            picker.FileTypeFilter.Add(".zip");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                StorageFolder d = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalCacheFolder.Path);
                await UnZipFileAync(file, d);
            }
            return Chapter;
        }


        public IAsyncAction UnZipFileAync(StorageFile zipFile, StorageFolder destinationFolder)
        {
            return UnZipFileHelper(zipFile, destinationFolder).AsAsyncAction();
        }

        private async Task UnZipFileHelper(StorageFile zipFile, StorageFolder destinationFolder)
        {
            if (zipFile == null || destinationFolder == null ||
                !Path.GetExtension(zipFile.FileType).Equals(".zip", StringComparison.CurrentCultureIgnoreCase)
                )
            {
                throw new ArgumentException("Invalid argument...");
            }
            Stream zipMemoryStream = await zipFile.OpenStreamForReadAsync();

            int pageNumber = 0;
            using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    var memStream = new MemoryStream();
                    await entry.Open().CopyToAsync(memStream);
                    memStream.Position = 0;
                    BitmapImage image = new BitmapImage();
                    image.SetSource(memStream.AsRandomAccessStream());
                    PageInfo info = new PageInfo
                    {
                        Image = image,
                        Number = pageNumber++,
                    };

                    Chapter.Pages.Add(info);
                }
            }
        }

        public async Task<ChapterInfo> LoadChaperFromLink(string uri)
        {
            try
            {
                Uri source = new Uri(uri);
                string fileName = Path.GetFileName(source.LocalPath) + ".zip";
                StorageFolder d = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalCacheFolder.Path);
                StorageFile destinationFile = await d.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(source, destinationFile);
                await download.StartAsync();
                await UnZipFileAync(destinationFile, d);

            }
            catch (Exception)
            {

                ErrorDialog dialog = new ErrorDialog();
                await dialog.ShowAsync();

            }
            return Chapter;

        }
    }
}
