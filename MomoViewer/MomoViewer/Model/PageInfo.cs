using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MomoViewer.Repository.Enums;

namespace MomoViewer.Model
{
    public class PageInfo
    {
        private int _number;
        private BitmapImage _image;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; }
        }
    }
}
