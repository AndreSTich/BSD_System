using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BSDSystem.API.Models;

public partial class BsdDbContext : DbContext
{
    public BsdDbContext()
    {
    }

    public BsdDbContext(DbContextOptions<BsdDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Duty> Duties { get; set; }

    public virtual DbSet<DutyRegistration> DutyRegistrations { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<Evente> Eventes { get; set; }

    public virtual DbSet<Explanatory> Explanatories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subdivision> Subdivisions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=BSD;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Duty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Duty__3214EC270433FB8F");

            entity.ToTable("Duty");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SubdivisionId).HasColumnName("Subdivision_ID");

            entity.HasOne(d => d.Subdivision).WithMany(p => p.Duties)
                .HasForeignKey(d => d.SubdivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Duty_Subdivision");
        });

        modelBuilder.Entity<DutyRegistration>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Duty_Registration");

            entity.Property(e => e.ApproverId).HasColumnName("Approver_ID");
            entity.Property(e => e.DutyId).HasColumnName("Duty_ID");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");

            entity.HasOne(d => d.Approver).WithMany()
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Duty_Registration_Approver");

            entity.HasOne(d => d.Duty).WithMany()
                .HasForeignKey(d => d.DutyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Duty_Registration_Duty");

            entity.HasOne(d => d.Employee).WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Duty_Registration_Employee");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC27E5706CD6");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.Logine, "UQ__Employee__D00D060061B12DD3").IsUnique();

            entity.HasIndex(e => e.BadgeNumber, "UQ__Employee__D110FD566DAA3034").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BadgeNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Logine)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Passworde)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Rating).HasDefaultValue(0);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.SubdivisionId).HasColumnName("Subdivision_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Role");

            entity.HasOne(d => d.Subdivision).WithMany(p => p.Employees)
                .HasForeignKey(d => d.SubdivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Subdivision");
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Event_Participant");

            entity.Property(e => e.ApproverId).HasColumnName("Approver_ID");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.EventId).HasColumnName("Event_ID");

            entity.HasOne(d => d.Approver).WithMany()
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Participant_Approver");

            entity.HasOne(d => d.Employee).WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Participant_Employee");

            entity.HasOne(d => d.Event).WithMany()
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Participant");
        });

        modelBuilder.Entity<Evente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evente__3214EC27059436D3");

            entity.ToTable("Evente");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatorId).HasColumnName("Creator_ID");
            entity.Property(e => e.Locations)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Ttype)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Creator).WithMany(p => p.Eventes)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evente_Creator");
        });

        modelBuilder.Entity<Explanatory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Explanat__3214EC27F6AE6FF0");

            entity.ToTable("Explanatory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApproverId).HasColumnName("Approver_ID");
            entity.Property(e => e.DisturberFirstName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Disturber_FirstName");
            entity.Property(e => e.DisturberLastName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Disturber_LastName");
            entity.Property(e => e.DisturberMiddleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Disturber_MiddleName");
            entity.Property(e => e.DisturberRoomNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Disturber_RoomNumber");
            entity.Property(e => e.DisturberType)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Disturber_Type");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");

            entity.HasOne(d => d.Approver).WithMany(p => p.ExplanatoryApprovers)
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Explanatory_Approver");

            entity.HasOne(d => d.Employee).WithMany(p => p.ExplanatoryEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Explanatory_Employee");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC27D01EEE87");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Subdivision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subdivis__3214EC27E17EBCC5");

            entity.ToTable("Subdivision");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
