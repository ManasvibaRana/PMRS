using Microsoft.EntityFrameworkCore;
using PMRS.Data;
using PMRS.Enums;

namespace PMRS.Services;

public class DashboardStats
{
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TodayAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int TotalMedicalRecords { get; set; }
    public int ActivePrescriptions { get; set; }
    public List<RecentAppointmentDto> RecentAppointments { get; set; } = new();
    public List<RecentPatientDto> RecentPatients { get; set; } = new();
    public List<RecentPrescriptionDto> RecentPrescriptions { get; set; } = new();
}

public class RecentPrescriptionDto
{
    public Guid Id { get; set; }
    public string PrescriptionNumber { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public PrescriptionStatus Status { get; set; }
}

public class RecentAppointmentDto
{
    public string AppointmentNumber { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public AppointmentStatus Status { get; set; }
}

public class RecentPatientDto
{
    public string PatientNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

public class DashboardService
{
    private readonly PMRSDbContext _db;

    public DashboardService(PMRSDbContext db) => _db = db;

    public async Task<DashboardStats> GetStatsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var stats = new DashboardStats
        {
            TotalPatients = await _db.Patients.CountAsync(),
            ActivePatients = await _db.Patients.CountAsync(p => p.IsActive),
            TotalDoctors = await _db.Doctors.CountAsync(d => d.IsActive),
            TodayAppointments = await _db.Appointments.CountAsync(a => a.ScheduledAt >= today && a.ScheduledAt < tomorrow),
            PendingAppointments = await _db.Appointments.CountAsync(a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed),
            TotalMedicalRecords = await _db.MedicalRecords.CountAsync(),
            ActivePrescriptions = await _db.Prescriptions.CountAsync(p => p.Status == PrescriptionStatus.Active)
        };
        stats.RecentAppointments = await _db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).OrderByDescending(a => a.CreatedAt).Take(5)
            .Select(a => new RecentAppointmentDto { AppointmentNumber = a.AppointmentNumber, PatientName = a.Patient.FirstName + " " + a.Patient.LastName, DoctorName = "Dr. " + a.Doctor.FirstName + " " + a.Doctor.LastName, ScheduledAt = a.ScheduledAt, Status = a.Status }).ToListAsync();
        stats.RecentPatients = await _db.Patients.OrderByDescending(p => p.RegisteredAt).Take(5)
            .Select(p => new RecentPatientDto { PatientNumber = p.PatientNumber, FullName = p.FirstName + " " + p.LastName, RegisteredAt = p.RegisteredAt }).ToListAsync();
        stats.RecentPrescriptions = await _db.Prescriptions.OrderByDescending(p => p.IssuedAt).Take(5)
            .Select(p => new RecentPrescriptionDto { Id = p.Id, PrescriptionNumber = p.PrescriptionNumber, PatientName = p.Patient.FirstName + " " + p.Patient.LastName, DoctorName = "Dr. " + p.Doctor.FirstName + " " + p.Doctor.LastName, IssuedAt = p.IssuedAt, Status = p.Status }).ToListAsync();
        return stats;
    }
}
