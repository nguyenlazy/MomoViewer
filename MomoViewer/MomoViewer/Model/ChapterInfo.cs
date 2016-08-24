using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
