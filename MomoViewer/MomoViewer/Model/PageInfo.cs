using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MomoViewer.Model
{
    public class PageInfo
    {
        private string _path;
        private int _number;
        private BitmapImage _image;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

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
