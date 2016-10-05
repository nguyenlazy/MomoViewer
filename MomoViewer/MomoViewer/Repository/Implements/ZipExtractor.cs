using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using MomoViewer.Controls;
using MomoViewer.Model;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.Implements
{
    class ZipExtractor : IExtractor
    {
        private DownloadProgress _extractProgress = new DownloadProgress();

        public async Task<List<BitmapImage>> Extract(string Uri)
        {
            _extractProgress.Title = "Extracting...";
            _extractProgress.ShowAsync();

            return await UnzipFileHelper(Uri);
        }

        private async Task<List<BitmapImage>> UnzipFileHelper(string uri)
        {   
            List<BitmapImage> _images = new List<BitmapImage>();

            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(uri);
            Stream zipStream = await zipFile.OpenStreamForReadAsync();
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                int total = zipArchive.Entries.Count;
                int current = 0;
                foreach (var entry in zipArchive.Entries)
                {
                    current++;
                    using (var memStream = new MemoryStream())
                    {
                        await entry.Open().CopyToAsync(memStream);
                        memStream.Position = 0;
                        BitmapImage image = new BitmapImage();
                        image.SetSource(memStream.AsRandomAccessStream());
                        _images.Add(image);
                    }

                    _extractProgress.Percent = current*100/total;
                    _extractProgress.PercentString = string.Format("{0}%", _extractProgress.Percent);
                    if (_extractProgress.Percent == 100)
                    {
                       _extractProgress.Hide();
                    }
                }
            }
            return _images;
        }
    }
}
