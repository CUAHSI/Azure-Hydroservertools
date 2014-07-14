using Microsoft.AspNet.Identity.EntityFramework;

namespace HydroServerTools.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string UserEmail { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
    public class EntityConnectionstringParameters
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