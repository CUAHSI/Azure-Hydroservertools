using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HydroServerTools.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public string UserEmail { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        public DbSet<ConnectionParameters> ConnectionParameters { get; set; }
        public DbSet<ConnectionParametersUser> ConnectionParametersUser { get; set; }

       

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
           

        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
        //    modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
        //    modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

        //    modelBuilder.Entity<User>().Property(c => c.UserName).IsRequired();
        //    modelBuilder.Entity<User>().Property(c => c.UserName).HasMaxLength(100);
        //}


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

    public class ConnectionParametersUser
    {
        public int Id { get; set; }
        [Display(Name = "Connection Name")]
        public int ConnectionParametersId { get; set; }
        [Display(Name = "User Name")]      
        public string UserId { get; set; }
        public virtual ConnectionParameters ConnectionParameters { get; set; }
        public virtual User User { get; set; }
    }
}