using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.Implements
{
    class FolderExtractor : IExtractor
    {
        public async Task<List<BitmapImage>> Extract(string Uri)
        {
            List<BitmapImage> images = new List<BitmapImage>();
            StorageFolder fd = await StorageFolder.GetFolderFromPathAsync(Uri);
            var fileList = await fd.GetFilesAsync();
            foreach (var storageFile in fileList)
            {

                if (storageFile.IsOfType(StorageItemTypes.File))
                {
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


                }
            }
            return images;
        }
    }
}
