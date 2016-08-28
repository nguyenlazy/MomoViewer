using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using MomoViewer.Controls;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.Implements
{
    class DRDownloader : IDownloader
    {
        private DownloadProgress _progress;
        private CancellationTokenSource _cancellationTokenSourceken = new CancellationTokenSource();

        private DownloadOperation _downloadOperation;
        private BackgroundDownloader _backgroundDownloader;

        public async Task<StorageFile> Download(string uri)
        {


            Uri source = new Uri(uri);
            string fileName = Path.GetFileName(source.LocalPath);
            StorageFolder d = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalCacheFolder.Path);

            StorageFile file = await d.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            _progress = new DownloadProgress();
            _progress.ShowAsync();
            _backgroundDownloader = new BackgroundDownloader();

            Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);

            _downloadOperation = _backgroundDownloader.CreateDownload(source, file);
            _downloadOperation.StartAsync();
            await _downloadOperation.AttachAsync().AsTask(_cancellationTokenSourceken.Token, progress);
            return file;
        }

        private void progressChanged(DownloadOperation dowloadOperation)
        {

            var byteRecieved = dowloadOperation.Progress.BytesReceived;
            var byteTotal = dowloadOperation.Progress.TotalBytesToReceive;
            _progress.Percent = (int)(100 * byteRecieved / byteTotal);
            _progress.PercentString = string.Format("{0}%", _progress.Percent);
            _progress.PrimaryButtonText = "Close";

            switch (_downloadOperation.Progress.Status)
            {
                case BackgroundTransferStatus.Idle:
                    break;
                case BackgroundTransferStatus.Running:
                    _progress.PrimaryButtonText = "Pause";
                    break;
                case BackgroundTransferStatus.PausedByApplication:
                case BackgroundTransferStatus.PausedCostedNetwork:
                    _progress.PrimaryButtonText = "Resume";
                    break;
                case BackgroundTransferStatus.PausedNoNetwork:
                    _progress.PercentString = "Network Error";
                    break;
                case BackgroundTransferStatus.Completed:
                    _progress.PercentString = "Completed";
                    break;
                case BackgroundTransferStatus.Canceled:
                    _progress.PercentString = "Canceled";
                    break;
                case BackgroundTransferStatus.Error:
                    break;
                case BackgroundTransferStatus.PausedSystemPolicy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_progress.Percent >= 100)
            {
                _progress.PercentString = "Download Completed";
                _progress.Hide();
            }
        }
    }
}
