using System;
using System.Collections;
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

        public void RemoveRecent()
        {
            using (var db = new DatabaseContext())
            {
                db.Links.RemoveRange(db.Links);
                db.SaveChanges();
            }
        }

        public IEnumerable<LinkInfo> GetAll()
        {

            IEnumerable<LinkInfo> result = new List<LinkInfo>();
            using (var db = new DatabaseContext())
            {
                result = db.Links.ToList();
            }
            return result;
        }
    }
}
