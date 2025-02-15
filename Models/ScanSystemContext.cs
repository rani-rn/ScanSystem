using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ScanBarcode.Models;

public partial class ScanSystemContext : DbContext
{
    public ScanSystemContext()
    {
    }

    public ScanSystemContext(DbContextOptions<ScanSystemContext> options)
        : base(options)
    {
    }

    public DbSet<DeliveryOrder> DeliveryOrders { get; set; }

    public DbSet<MasterItem> MasterItems { get; set; }

    public DbSet<ProdModel> ProdModels { get; set; }

    public DbSet<MasterTable> MasterTables { get; set; }

    public DbSet<Rfidtag> Rfidtags { get; set; }

    public DbSet<Shipment> Shipments { get; set; }

    public DbSet<TempScanItem> TempScanItems { get; set; }
}

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=192.168.43.180,1434;Database=ScanSystem;User=sa;Password=sa;TrustServerCertificate=True");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<DeliveryOrder>(entity =>
//        {
//            entity.HasKey(e => e.Doid);

//            entity.HasIndex(e => e.ModelId, "IX_DeliveryOrders_ModelId");

//            entity.Property(e => e.Doid).HasColumnName("DOId");
//            entity.Property(e => e.Destination)
//                .HasMaxLength(50)
//                .IsFixedLength();
//            entity.Property(e => e.Donumber)
//                .HasMaxLength(20)
//                .HasColumnName("DONumber");

//            entity.HasOne(d => d.Model).WithMany(p => p.DeliveryOrders)
//                .HasForeignKey(d => d.ModelId)
//                .OnDelete(DeleteBehavior.ClientSetNull);
//        });

//        modelBuilder.Entity<MasterItem>(entity =>
//        {
//            entity.HasKey(e => e.ItemId);

//            entity.HasIndex(e => e.Model, "IX_MasterItems_ModelId");

//            entity.HasIndex(e => e.RfidtagId, "IX_MasterItems_RFIDTagId");

//            entity.Property(e => e.RfidtagId).HasColumnName("RFIDTagId");
//            entity.Property(e => e.SerialNumber).HasMaxLength(25);

//            entity.HasOne(d => d.Model).WithMany(p => MasterItems)
//                .HasForeignKey(d => d.Model)
//                .OnDelete(DeleteBehavior.ClientSetNull);

//            entity.HasOne(d => d.Rfidtag).WithMany(p => p.MasterItems)
//                .HasForeignKey(d => d.RfidtagId)
//                .OnDelete(DeleteBehavior.ClientSetNull);
//        });

//        modelBuilder.Entity<ProdModel>(entity =>
//        {
//            entity.Property(e => e.ModelName).HasMaxLength(50);
//        });

//        modelBuilder.Entity<MasterTable>(entity =>
//        {
//            entity.HasIndex(e => e.Model, "IX_Productions_ModelId");

//            entity.Property(e => e.SerialNumber).HasMaxLength(25);

//        });

//        modelBuilder.Entity<Rfidtag>(entity =>
//        {
//            entity.ToTable("RFIDTags");

//            entity.HasIndex(e => e.Model, "IX_RFIDTags_ModelId");

//            entity.Property(e => e.Id).HasColumnName("Id");

//        });

//        modelBuilder.Entity<Shipment>(entity =>
//        {
//            entity.HasIndex(e => e.Doid, "IX_Shipments_DOId");

//            entity.HasIndex(e => e.RfidtagId, "IX_Shipments_RFIDTagId");

//            entity.Property(e => e.Destination).HasMaxLength(50);
//            entity.Property(e => e.Doid).HasColumnName("DOId");
//            entity.Property(e => e.RfidtagId).HasColumnName("RFIDTagId");

//            entity.HasOne(d => d.Do).WithMany(p => p.Shipments)
//                .HasForeignKey(d => d.Doid)
//                .OnDelete(DeleteBehavior.ClientSetNull);

//            entity.HasOne(d => d.Rfidtag).WithMany(p => p.Shipments)
//                .HasForeignKey(d => d.RfidtagId)
//                .OnDelete(DeleteBehavior.ClientSetNull);
//        });

//        modelBuilder.Entity<TempScanItem>(entity =>
//        {
//            entity.HasKey(e => e.TempScanId);

//            entity.HasIndex(e => e.ModelId, "IX_TempScanItems_ModelId");

//            entity.Property(e => e.SerialNumber).HasMaxLength(25);

//            entity.HasOne(d => d.Model).WithMany(p => p.TempScanItems)
//                .HasForeignKey(d => d.ModelId)
//                .OnDelete(DeleteBehavior.ClientSetNull);
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
