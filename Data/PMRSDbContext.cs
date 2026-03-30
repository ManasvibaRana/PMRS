using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMRS.Entities;

namespace PMRS.Data;

public class PMRSDbContext : IdentityDbContext<ApplicationUser>
{
    public PMRSDbContext(DbContextOptions<PMRSDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        // Patient
        b.Entity<Patient>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.PatientNumber).IsRequired().HasMaxLength(20);
            e.HasIndex(p => p.PatientNumber).IsUnique();
            e.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            e.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            e.Property(p => p.Email).IsRequired().HasMaxLength(200);
            e.Property(p => p.Phone).IsRequired().HasMaxLength(20);
            e.OwnsOne(p => p.Address, a =>
            {
                a.Property(x => x.Street).HasMaxLength(200).HasColumnName("AddressStreet");
                a.Property(x => x.City).HasMaxLength(100).HasColumnName("AddressCity");
                a.Property(x => x.State).HasMaxLength(100).HasColumnName("AddressState");
                a.Property(x => x.PostalCode).HasMaxLength(20).HasColumnName("AddressPostalCode");
                a.Property(x => x.Country).HasMaxLength(100).HasColumnName("AddressCountry");
            });
            e.Ignore(p => p.FullName);
            e.Ignore(p => p.Age);
        });
        // Doctor
        b.Entity<Doctor>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
            e.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            e.Property(d => d.Specialisation).IsRequired().HasMaxLength(200);
            e.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
            e.HasIndex(d => d.LicenseNumber).IsUnique();
            e.Property(d => d.Email).IsRequired().HasMaxLength(200);
            e.Property(d => d.Phone).IsRequired().HasMaxLength(20);
            e.Ignore(d => d.FullName);
        });
        // Appointment
        b.Entity<Appointment>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.AppointmentNumber).IsRequired().HasMaxLength(20);
            e.HasIndex(a => a.AppointmentNumber).IsUnique();
            e.Property(a => a.ChiefComplaint).IsRequired().HasMaxLength(500);
            e.HasOne(a => a.Patient).WithMany(p => p.Appointments).HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Doctor).WithMany(d => d.Appointments).HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);
        });
        // MedicalRecord
        b.Entity<MedicalRecord>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Title).IsRequired().HasMaxLength(200);
            e.HasOne(m => m.Patient).WithMany(p => p.MedicalRecords).HasForeignKey(m => m.PatientId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Doctor).WithMany(d => d.MedicalRecords).HasForeignKey(m => m.DoctorId).OnDelete(DeleteBehavior.Restrict);
        });
        // Prescription
        b.Entity<Prescription>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.PrescriptionNumber).IsRequired().HasMaxLength(20);
            e.HasIndex(p => p.PrescriptionNumber).IsUnique();
            e.HasOne(p => p.Patient).WithMany(p => p.Prescriptions).HasForeignKey(p => p.PatientId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Doctor).WithMany(d => d.Prescriptions).HasForeignKey(p => p.DoctorId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
