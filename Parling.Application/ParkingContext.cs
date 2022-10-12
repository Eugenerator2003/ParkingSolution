using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Parking.Domain
{
    public partial class ParkingContext : DbContext
    {
        public ParkingContext()
        {
        }

        public ParkingContext(DbContextOptions<ParkingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarMark> CarMarks { get; set; } = null!;
        public virtual DbSet<Emploeyee> Emploeyees { get; set; } = null!;
        public virtual DbSet<Owner> Owners { get; set; } = null!;
        public virtual DbSet<ParkingRecord> ParkingRecords { get; set; } = null!;
        public virtual DbSet<ParkingType> ParkingTypes { get; set; } = null!;
        public virtual DbSet<PaymentTariff> PaymentTariffs { get; set; } = null!;
        public virtual DbSet<WorkShift> WorkShifts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-SRISG2B\\SQLEXPRESS;Database=Parking;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CarMarkId).HasColumnName("carMarkId");

                entity.Property(e => e.Number)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("number");

                entity.HasOne(d => d.CarMark)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.CarMarkId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Cars__carMarkId__2D27B809");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Cars__OwnerId__2E1BDC42");
            });

            modelBuilder.Entity<CarMark>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Emploeyee>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("fullname");
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("fullname");

                entity.Property(e => e.PhoneNumber).HasColumnName("phoneNumber");
            });

            modelBuilder.Entity<ParkingRecord>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CarId).HasColumnName("carId");

                entity.Property(e => e.DepartureTime)
                    .HasColumnType("datetime")
                    .HasColumnName("departureTime");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.EntryTime)
                    .HasColumnType("datetime")
                    .HasColumnName("entryTime");

                entity.Property(e => e.PaymentTariffIdId).HasColumnName("paymentTariffIdId");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.ParkingRecords)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__ParkingRe__carId__35BCFE0A");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ParkingRecords)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__ParkingRe__emplo__37A5467C");

                entity.HasOne(d => d.PaymentTariffId)
                    .WithMany(p => p.ParkingRecords)
                    .HasForeignKey(d => d.PaymentTariffIdId)
                    .HasConstraintName("FK__ParkingRe__payme__36B12243");
            });

            modelBuilder.Entity<ParkingType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(52)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<PaymentTariff>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DaysCount).HasColumnName("daysCount");

                entity.Property(e => e.ParkingTypeId).HasColumnName("parkingTypeId");

                entity.Property(e => e.Payment).HasColumnType("money");

                entity.HasOne(d => d.ParkingType)
                    .WithMany(p => p.PaymentTariffs)
                    .HasForeignKey(d => d.ParkingTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__PaymentTa__parki__267ABA7A");
            });

            modelBuilder.Entity<WorkShift>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EmploeyeeId).HasColumnName("emploeyeeId");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("endTime");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("startTime");

                entity.HasOne(d => d.Emploeyee)
                    .WithMany(p => p.WorkShifts)
                    .HasForeignKey(d => d.EmploeyeeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__WorkShift__emplo__32E0915F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
