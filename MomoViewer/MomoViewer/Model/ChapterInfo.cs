using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MomoViewer.Model
{
    public class ChapterInfo
    {
        private string _name;
        private int _number;
        private List<PageInfo> pages;

        public ChapterInfo()
        {
            pages = new List<PageInfo>();
        }
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<PageInfo> Pages
        {
            get { return pages; }
            set { pages = value; }
        }

        //public IEnumerable<string> Urls
        //{
        //    get
        //    {
        //        return from pageInfo in pages
        //               where pageInfo.Path.Contains("jpeg") || pageInfo.Path.Contains(".png") || pageInfo.Path.Contains(".jpg") || pageInfo.Path.Contains(".gif")
        //               select pageInfo.Path;

        //    }
        //}

        public IEnumerable<BitmapImage> Images
        {
            get
            {
                return from pageInfo in pages
                       select pageInfo.Image;
            }
        }
    }
}
