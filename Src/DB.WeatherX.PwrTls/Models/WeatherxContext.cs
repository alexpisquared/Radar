﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DB.WeatherX.PwrTls.Models
{
    public partial class WeatherxContext : DbContext
    {
        public WeatherxContext(DbContextOptions<WeatherxContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ForeVsReal> ForeVsReal { get; set; } = null!;
        public virtual DbSet<LkuMeasure> LkuMeasure { get; set; } = null!;
        public virtual DbSet<LkuSite> LkuSite { get; set; } = null!;
        public virtual DbSet<LkuSrc> LkuSrc { get; set; } = null!;
        public virtual DbSet<PointFore> PointFore { get; set; } = null!;
        public virtual DbSet<PointReal> PointReal { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ForeVsReal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DevDbgDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DevDbgDtTm2).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DevDbgDtoffset)
                    .HasColumnName("DevDbgDTOffset")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ForeSiteId)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.ForecastedAt).HasColumnType("datetime");

                entity.Property(e => e.ForecastedFor).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.RealSiteId)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<LkuMeasure>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FreeNtext)
                    .HasColumnType("ntext")
                    .HasColumnName("FreeNText");

                entity.Property(e => e.Name32).HasMaxLength(32);
            });

            modelBuilder.Entity<LkuSite>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FreeNtext)
                    .HasColumnType("ntext")
                    .HasColumnName("FreeNText");

                entity.Property(e => e.Name32).HasMaxLength(32);
            });

            modelBuilder.Entity<LkuSrc>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FreeNtext)
                    .HasColumnType("ntext")
                    .HasColumnName("FreeNText");

                entity.Property(e => e.Name32).HasMaxLength(32);
            });

            modelBuilder.Entity<PointFore>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FreeNtext)
                    .HasColumnType("ntext")
                    .HasColumnName("FreeNText");

                entity.Property(e => e.MeasureId)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.Note64).HasMaxLength(64);

                entity.Property(e => e.SiteId)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.SrcId)
                    .HasMaxLength(3)
                    .IsFixedLength();
            });

            modelBuilder.Entity<PointReal>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FreeNtext)
                    .HasColumnType("ntext")
                    .HasColumnName("FreeNText");

                entity.Property(e => e.MeasureId)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.Note64).HasMaxLength(64);

                entity.Property(e => e.SiteId)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.SrcId)
                    .HasMaxLength(3)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}