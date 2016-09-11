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
using MomoViewer.Model;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.Implements
{
    class ZipExtractor : IExtractor
    {

        public async Task<List<BitmapImage>> Extract(string Uri)
        {
            return await UnzipFileHelper(Uri);
        }

        private async Task<List<BitmapImage>> UnzipFileHelper(string uri)
        {
            List<BitmapImage> _images = new List<BitmapImage>();

            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(uri);
            Stream zipStream = await zipFile.OpenStreamForReadAsync();
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    using (var memStream = new MemoryStream())
                    {
                        await entry.Open().CopyToAsync(memStream);
                        memStream.Position = 0;
                        BitmapImage image = new BitmapImage();
                        image.SetSource(memStream.AsRandomAccessStream());
                        _images.Add(image);
                    }
                }
            }
            return _images;
        }
    }
}
