using System;
using APIsASManager.CoreV3.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace APIsASManager.CoreV3.Models.EFModels
{
    public partial class TALogContext : DbContext
    {
        Guid guid;
        static string connString;
        IConfiguration config;
        public TALogContext() :base()
        {
            //guid = Guid.NewGuid();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            connString = configuration.GetConnectionString("TALogConnection").ToString();
        }

        public TALogContext(DbContextOptions<TALogContext> options)
            : base(options)
        {
            var vTemp = options.FindExtension<SqlServerOptionsExtension>();
            if (vTemp != null)
            {
                connString = vTemp.ConnectionString;
            }
        }
        public virtual DbSet<ACCESS_GRANTED> ACCESS_GRANTEDS { get; set; }
        public virtual DbSet<RECORD_DATA> RECORD_DATAS { get; set; }
        public virtual DbSet<VIEW_RECORD_DATA> VIEW_RECORD_DATAS { get; set; }
        public virtual DbSet<EXCEPTION> EXCEPTIONS { get; set; }
        public virtual DbSet<HEART_BREAK> HEART_BREAKS { get; set; }
        public virtual DbSet<CHECK_OPEN_AUTH> CHECK_OPEN_AUTHS { get; set; }
        public virtual DbSet<FACE_SYNC> FACE_SYNCS { get; set; }
        public virtual DbSet<VIEW_TEMPERATURE_RECORD> VIEW_TEMPERATURE_RECORDS { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false);

                var configuration = builder.Build();

                connString = configuration.GetConnectionString("TALogConnection").ToString();
                optionsBuilder.UseSqlServer(connString);
            }
            //optionsBuilder.UseSqlServer(connString.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Thai_CI_AS");

            modelBuilder.Entity<ACCESS_GRANTED>(entity =>
            {
                entity.HasKey(e => new { e.log_id, e.log_time });

                entity.ToTable("ACCESS_GRANTED", "dbo");

            });

            modelBuilder.Entity<RECORD_DATA>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("RECORD_DATA", "rakinda");
            });

            modelBuilder.Entity<VIEW_RECORD_DATA>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("ViewRECORD_DATA", "rakinda");
            });

            modelBuilder.Entity<EXCEPTION>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("EXCEPTION", "rakinda");
            });

            modelBuilder.Entity<HEART_BREAK>(entity =>
            {
                entity.HasKey(e => new { e.device_no});

                entity.ToTable("HEART_BREAK", "rakinda");

            });

            modelBuilder.Entity<CHECK_OPEN_AUTH>(entity =>
            {
                entity.HasKey(e => new { e.device_no, e.qrcode });

                entity.ToTable("CHECK_OPEN_AUTH", "rakinda");

            });

            modelBuilder.Entity<FACE_SYNC>(entity =>
            {
                entity.HasKey(e => new { e.userId });

                entity.ToTable("FACE_SYNC", "rakinda");

            });

            modelBuilder.Entity<VIEW_TEMPERATURE_RECORD>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("ViewTEMPERATURE_RECORD", "rakinda");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
