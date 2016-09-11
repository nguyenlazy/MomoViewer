using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Practices.Unity;
using MomoViewer.Model;
using MomoViewer.Repository;
using MomoViewer.Repository.Implements;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.ViewModel
{
    public class ChapterVM
    {
        private IReader _reader;
        private int _totalPages;

        public int TotalPages
        {
            get { return _totalPages; }
            set { _totalPages = value; }
        }

        public ChapterVM(IReader reader)
        {
            _reader = reader;
        }


        public async Task<ObservableCollection<BitmapImage>> GetBitmapImages(LinkInfo link)
        {
            var info = await _reader.Read(link);
            ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>(info.Images);
            _totalPages = images.Count;
            return images;
        }


    }
}
