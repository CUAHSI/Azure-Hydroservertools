using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchronizeData
{
    public class UpdateDatabasesModel
    {        
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public int ConnectionId { get; set; }
    }

    public class ConnectionParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        //public virtual ICollection<User> User { get; set; }
        //public virtual newtable2 nt2 {get; set;}
    }
}
