using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BDMPro.Models;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace BDMPro.Data
{
    //commented original code
    //public class DefaultDBContext : IdentityDbContext
    //{
    //    public DefaultDBContext(DbContextOptions<DefaultDBContext> options)
    //        : base(options)
    //    {
    //    }
    //}

    public class DefaultDBContext : IdentityDbContext<AspNetUsers, AspNetRoles, string,
                                        AspNetUserClaims, AspNetUserRoles, AspNetUserLogins, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DefaultDBContext(DbContextOptions<DefaultDBContext> options)
            : base(options)
        {
        }

        //used by asp.net identity
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }

        //other tables
        public DbSet<GlobalOptionSet> GlobalOptionSets { get; set; }
        public DbSet<BDMPro.Models.Module> Modules { get; set; }
        public DbSet<RoleModulePermission> RoleModulePermissions { get; set; }
        public DbSet<UserAttachment> UserAttachments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.RemovePluralizingTableNameConvention();
            builder.Entity<AspNetUsers>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
            base.OnModelCreating(builder);
        }
    }

}