using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomoViewer.Model
{
    public class LinkInfo
    {
        public int LinkInfoId { get; set; }
        string _path;
        private LinkType _type;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public LinkType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }

    public enum LinkType
    {
        OfflineZip = 0,
        OfflineFolder = 1,
        Online = 2
    }
}
