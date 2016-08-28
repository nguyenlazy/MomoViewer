using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MomoViewer.Repository.Enums;

namespace MomoViewer.Model
{
    public class ChapterInfo
    {
        private string _name;
        private int _number;
        private string _mangaPath;
        private List<PageInfo> pages;
        private MangaType _readType;


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


        public IEnumerable<BitmapImage> Images
        {
            get
            {
                return from pageInfo in pages
                       select pageInfo.Image;
            }
        }

        public MangaType ReadType
        {
            get { return _readType; }
            set { _readType = value; }
        }

        public string MangaPath
        {
            get { return _mangaPath; }
            set { _mangaPath = value; }
        }
    }
}
