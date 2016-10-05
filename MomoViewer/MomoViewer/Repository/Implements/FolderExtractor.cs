using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using MomoViewer.Controls;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.Implements
{
    class FolderExtractor : IExtractor
    {
        private DownloadProgress _progress = new DownloadProgress();
        public async Task<List<BitmapImage>> Extract(string Uri)
        {
            _progress.Title = "Reading ...";
            _progress.ShowAsync();
            int numOfFile = 0;
            List<BitmapImage> images = new List<BitmapImage>();
            StorageFolder fd = await StorageFolder.GetFolderFromPathAsync(Uri);
            var fileList = await fd.GetFilesAsync();
            int totalFile = fileList.Count;
            foreach (var storageFile in fileList)
            {

                if (storageFile.IsOfType(StorageItemTypes.File))
                {
                    numOfFile++;
                    string mediaType = storageFile.ContentType.Split('/')[0];
                    if (mediaType == "image")
                    {
                        using (var stream = await storageFile.OpenAsync(FileAccessMode.Read))
                        {
                            var bitmapImage = new BitmapImage();
                            await bitmapImage.SetSourceAsync(stream);
                            images.Add(bitmapImage);
                        }
                    }

                    _progress.Percent = numOfFile * 100 / totalFile;
                    _progress.PercentString = string.Format("{0} / {1}", numOfFile, totalFile);

                    if (_progress.Percent == 100)
                    {
                        _progress.Hide();
                    }
                }
            }
            return images;
        }
    }
}
