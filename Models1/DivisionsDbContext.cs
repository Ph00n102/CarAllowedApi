using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CarAllowedApi.Models1;

public partial class DivisionsDbContext : DbContext
{
    public DivisionsDbContext()
    {
    }

    public DivisionsDbContext(DbContextOptions<DivisionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Divisions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=172.16.200.202;port=3307;database=divisiondb;user=root;password=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.11-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ChiefName).HasMaxLength(200);
            entity.Property(e => e.Cid).HasMaxLength(13);
            entity.Property(e => e.DivisionName).HasMaxLength(200);
            entity.Property(e => e.LoginNameCustom).HasMaxLength(100);
            entity.Property(e => e.LoginNameHosxp).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
