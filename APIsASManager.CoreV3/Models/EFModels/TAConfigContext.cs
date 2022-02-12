using System;
using APIsASManager.CoreV3.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace APIsASManager.CoreV3.Models.EFModels
{
    public partial class TAConfigContext : DbContext
    {
        Guid guid;
        static string connString;
        IConfiguration config;
        public TAConfigContext() :base()
        {
            //guid = Guid.NewGuid();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            connString = configuration.GetConnectionString("TALogConnection").ToString();
        }

        public TAConfigContext(DbContextOptions<TAConfigContext> options)
            : base(options)
        {
            var vTemp = options.FindExtension<SqlServerOptionsExtension>();
            if (vTemp != null)
            {
                connString = vTemp.ConnectionString;
            }
        }
        public virtual DbSet<CARD_NO_LIST> CARD_NO_LISTS { get; set; }
        public virtual DbSet<GET_CARD> GET_CARDS { get; set; }
        public virtual DbSet<GET_USER> GET_USERS { get; set; }
        public virtual DbSet<USER_ID_LIST> USER_ID_LISTS { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false);

                var configuration = builder.Build();

                connString = configuration.GetConnectionString("TAConfigConnection").ToString();
                optionsBuilder.UseSqlServer(connString);
            }
            //optionsBuilder.UseSqlServer(connString.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Thai_CI_AS");

            modelBuilder.Entity<CARD_NO_LIST>(entity =>
            {
                entity.HasKey(e => new { e.c_code, e.c_no });

                entity.ToTable("CARD_NO_LIST", "dbo");

            });

            modelBuilder.Entity<USER_ID_LIST>(entity =>
            {
                entity.HasKey(e => new { e.ch_id, e.ch_name });

                entity.ToTable("USER_ID_LIST", "dbo");

            });

            modelBuilder.Entity<GET_CARD>(entity =>
            {
                entity.HasKey(e => new { e.ch_id, e.c_code, e.c_no });

                entity.ToTable("GET_CARD", "dbo");

            });

            modelBuilder.Entity<GET_USER>(entity =>
            {
                entity.HasKey(e => new { e.ch_id });

                entity.ToTable("GET_USER", "dbo");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
