using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomoViewer.Model;

namespace MomoViewer.Repository.Interfaces
{
    interface IReader
    {
        Task<ChapterInfo> Read(LinkInfo Uri);
    }
}
