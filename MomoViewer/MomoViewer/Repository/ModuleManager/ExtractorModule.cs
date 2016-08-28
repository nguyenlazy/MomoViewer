using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomoViewer.Model;
using MomoViewer.Repository.Enums;
using MomoViewer.Repository.Implements;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.ModuleManager
{
    class ExtractorModule
    {
        public static Dictionary<MangaType, IExtractor> Module = new Dictionary<MangaType, IExtractor>
        {
            {MangaType.Folder, new FolderExtractor() },
            {MangaType.Zip, new ZipExtractor() },
        };
    }
}
