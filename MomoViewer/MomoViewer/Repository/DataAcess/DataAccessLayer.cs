using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomoViewer.Model;

namespace MomoViewer.Repository.DataAcess
{
    public class DataAccessLayer : IDataAccess
    {
        public void AddRecent(LinkInfo link)
        {
            using (var db = new DatabaseContext())
            {
                db.Links.Add(link);
                db.SaveChanges();
            }
        }
    }
}
