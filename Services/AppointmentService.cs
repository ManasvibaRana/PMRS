using Microsoft.EntityFrameworkCore;
using PMRS.Data;
using PMRS.Entities;
using PMRS.Enums;

namespace PMRS.Services;

public class AppointmentService
{
    private readonly PMRSDbContext _db;

    public AppointmentService(PMRSDbContext db) => _db = db;

    public async Task<PaginatedList<Appointment>> GetListAsync(DateTime? from, DateTime? to, int page = 1, int pageSize = 10)
    {
        IQueryable<Appointment> q = _db.Appointments.AsNoTracking().Include(a => a.Patient).Include(a => a.Doctor);
        if (from.HasValue) q = q.Where(a => a.ScheduledAt >= from.Value);
        if (to.HasValue) q = q.Where(a => a.ScheduledAt < to.Value.AddDays(1));
        q = q.OrderByDescending(a => a.ScheduledAt);
        return await PaginatedList<Appointment>.CreateAsync(q, page, pageSize);
    }

    public async Task<Appointment?> GetByIdAsync(Guid id) =>
        await _db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Appointment> CreateAsync(Guid patientId, Guid doctorId, DateTime scheduledAt, int durationMinutes, string chiefComplaint, string? notes)
    {
        var next = await _db.Appointments.CountAsync() + 1;
        var apt = new Appointment { AppointmentNumber = $"APT-{next:D6}", PatientId = patientId, DoctorId = doctorId, ScheduledAt = scheduledAt, DurationMinutes = durationMinutes, ChiefComplaint = chiefComplaint ?? "", Notes = notes, Status = AppointmentStatus.Scheduled, CreatedAt = DateTime.UtcNow };
        _db.Appointments.Add(apt);
        await _db.SaveChangesAsync();
        return apt;
    }

    public async Task<bool> UpdateAsync(Guid id, DateTime scheduledAt, int durationMinutes, string chiefComplaint, string? notes, AppointmentStatus status)
    {
        var a = await _db.Appointments.FindAsync(id);
        if (a == null) return false;
        a.ScheduledAt = scheduledAt; a.DurationMinutes = durationMinutes; a.ChiefComplaint = chiefComplaint ?? ""; a.Notes = notes; a.Status = status; a.UpdatedAt = DateTime.UtcNow;
        if (status == AppointmentStatus.Cancelled) a.CancelledAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid id, string? reason)
    {
        var a = await _db.Appointments.FindAsync(id);
        if (a == null) return false;
        a.Status = AppointmentStatus.Cancelled; a.CancelledAt = DateTime.UtcNow; a.CancellationReason = reason; a.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
