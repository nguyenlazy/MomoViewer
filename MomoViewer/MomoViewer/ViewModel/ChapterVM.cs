using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Audio;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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

        private DownloadOperation _downloadOperation;
        private BackgroundDownloader _backgroundDownloader;
        private CancellationTokenSource _cancellationTokenSource;
        public ChapterVM()
        {
            _chapter = new ChapterInfo();
            _backgroundDownloader = new BackgroundDownloader();



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

        private DownloadProgress dialog;
        public async Task<ChapterInfo> LoadChapterFromLink(string uri)
        {
            try
            {

                dialog = new DownloadProgress();

                dialog.ShowAsync();


                Uri source = new Uri(uri);
                string fileName = Path.GetFileName(source.LocalPath) + ".zip";
                StorageFolder d = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalCacheFolder.Path);
                StorageFile destinationFile = await d.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                _downloadOperation = _backgroundDownloader.CreateDownload(source, destinationFile);
                Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);
                _downloadOperation.StartAsync();
                dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
                _cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    await _downloadOperation.AttachAsync().AsTask(_cancellationTokenSource.Token, progress);
                    await UnZipFileAync(destinationFile, d);
                    return Chapter;

                }
                catch (TaskCanceledException)
                {

                    _downloadOperation.Pause();
                    dialog.PrimaryButtonText = "Resume";
                    await dialog.ShowAsync();
                }

            }
            catch (Exception e)
            {
                MessageDialog dialog = new MessageDialog(e.Message, "Error");
                await dialog.ShowAsync();

            }

            return Chapter;

        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (dialog.PrimaryButtonText == "Pause")
            {
                _cancellationTokenSource.Cancel();

            }
            else if (dialog.PrimaryButtonText == "Resume")
            {
                _downloadOperation.Resume();
            }
            else
            {
                sender.Hide();
            }
        }

        private void progressChanged(DownloadOperation dowloadOperation)
        {

            var byteRecieved = dowloadOperation.Progress.BytesReceived;
            var byteTotal = dowloadOperation.Progress.TotalBytesToReceive;
            dialog.Percent = (int)(100 * byteRecieved / byteTotal);
            dialog.PercentString = string.Format("{0}%", dialog.Percent);
            dialog.PrimaryButtonText = "Close";

            switch (_downloadOperation.Progress.Status)
            {
                case BackgroundTransferStatus.Idle:
                    break;
                case BackgroundTransferStatus.Running:
                    dialog.PrimaryButtonText = "Pause";
                    break;
                case BackgroundTransferStatus.PausedByApplication:
                case BackgroundTransferStatus.PausedCostedNetwork:
                    dialog.PrimaryButtonText = "Resume";
                    break;
                case BackgroundTransferStatus.PausedNoNetwork:
                    dialog.PercentString = "Network Error";
                    break;
                case BackgroundTransferStatus.Completed:
                    dialog.PercentString = "Completed";
                    break;
                case BackgroundTransferStatus.Canceled:
                    dialog.PercentString = "Canceled";
                    break;
                case BackgroundTransferStatus.Error:
                    break;
                case BackgroundTransferStatus.PausedSystemPolicy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (dialog.Percent >= 100)
            {
                dialog.PercentString = "Download Completed";
                dialog.Hide();
            }
        }
    }
}
