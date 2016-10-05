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

        public string _name;

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

        public string Name
        {
            get
            {
                _name = System.IO.Path.GetFileNameWithoutExtension(_path);
                if (string.IsNullOrEmpty(_name))
                {
                    _name = System.IO.Path.GetDirectoryName(_path);
                }
                return _name;
            }
            set { _name = value; }
        }
    }

    public enum LinkType
    {
        OfflineZip = 0,
        OfflineFolder = 1,
        Online = 2
    }
}
