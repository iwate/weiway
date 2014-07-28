using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WeiWay.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore]
        public override string PasswordHash
        {
            get
            {
                return base.PasswordHash;
            }
            set
            {
                base.PasswordHash = value;
            }
        }
        [JsonIgnore]
        public override string SecurityStamp
        {
            get
            {
                return base.SecurityStamp;
            }
            set
            {
                base.SecurityStamp = value;
            }
        }
        [JsonIgnore]
        public override int AccessFailedCount
        {
            get
            {
                return base.AccessFailedCount;
            }
            set
            {
                base.AccessFailedCount = value;
            }
        }
        [JsonIgnore]
        public override ICollection<IdentityUserClaim> Claims
        {
            get
            {
                return base.Claims;
            }
        }
        [JsonIgnore]
        public override string Email
        {
            get
            {
                return base.Email;
            }
            set
            {
                base.Email = value;
            }
        }
        [JsonIgnore]
        public override bool EmailConfirmed
        {
            get
            {
                return base.EmailConfirmed;
            }
            set
            {
                base.EmailConfirmed = value;
            }
        }
         [JsonIgnore]
        public override ICollection<IdentityUserLogin> Logins
        {
            get
            {
                return base.Logins;
            }
        }
         [JsonIgnore]
        public override bool LockoutEnabled
        {
            get
            {
                return base.LockoutEnabled;
            }
            set
            {
                base.LockoutEnabled = value;
            }
        }
         [JsonIgnore]
        public override System.DateTime? LockoutEndDateUtc
        {
            get
            {
                return base.LockoutEndDateUtc;
            }
            set
            {
                base.LockoutEndDateUtc = value;
            }
        }
         [JsonIgnore]
        public override string PhoneNumber
        {
            get
            {
                return base.PhoneNumber;
            }
            set
            {
                base.PhoneNumber = value;
            }
        }
         [JsonIgnore]
        public override bool PhoneNumberConfirmed
        {
            get
            {
                return base.PhoneNumberConfirmed;
            }
            set
            {
                base.PhoneNumberConfirmed = value;
            }
        }
         [JsonIgnore]
        public override ICollection<IdentityUserRole> Roles
        {
            get
            {
                return base.Roles;
            }
        }
        [JsonIgnore]
        public override bool TwoFactorEnabled
        {
            get
            {
                return base.TwoFactorEnabled;
            }
            set
            {
                base.TwoFactorEnabled = value;
            }
        }
        public string Icon { get; set; }
        [JsonIgnore]
        public virtual ICollection<Wei> WeiWei { get; set; }
        [JsonIgnore]
        public virtual ICollection<Message> Messages { get; set; }
        [JsonIgnore]
        public virtual ICollection<Wei> Onechances { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public ApplicationUser()
        {
            WeiWei = new HashSet<Wei>();
            Onechances = new HashSet<Wei>();
            Messages = new HashSet<Message>();
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Wei> WeiWei { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.WeiWei).WithOptional(x => x.User);
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Messages).WithMany(x => x.Users).Map(x => x.ToTable("MessageUser"));
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Onechances).WithMany(x => x.OneChanceUsers).Map(x => x.ToTable("OnechanceUser"));
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.WeiWei).WithRequired(x => x.User).WillCascadeOnDelete(false);
            modelBuilder.Entity<Wei>().HasRequired<Message>(x => x.Message);
            //modelBuilder.Entity<Wei>().HasMany(x => x.OneChanceUsers).WithMany(x => x.WeiWei).Map(x => x.ToTable("UserWei"));
            modelBuilder.Entity<Wei>().HasMany(x => x.Comments).WithRequired(x => x.Wei).WillCascadeOnDelete(false);
            modelBuilder.Entity<Comment>().HasRequired<Message>(x => x.Message);
            base.OnModelCreating(modelBuilder);
        }
    }
}