using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomoViewer.Model;

namespace MomoViewer.Repository.DataAcess
{
    public interface IDataAccess
    {
        void AddRecent(LinkInfo info);
        void RemoveRecent();
        IEnumerable<LinkInfo> GetAll();
    }
}
