using Microsoft.EntityFrameworkCore;
using PMRS.Data;
using PMRS.Entities;
using PMRS.Enums;

namespace PMRS.Services;

public class PrescriptionService
{
    private readonly PMRSDbContext _db;

    public PrescriptionService(PMRSDbContext db) => _db = db;

    public async Task<PaginatedList<Prescription>> GetListAsync(Guid? patientId, PrescriptionStatus? status, int page = 1, int pageSize = 10)
    {
        IQueryable<Prescription> q = _db.Prescriptions.AsNoTracking().Include(p => p.Patient).Include(p => p.Doctor).Include(p => p.Items);
        if (patientId.HasValue) q = q.Where(p => p.PatientId == patientId.Value);
        if (status.HasValue) q = q.Where(p => p.Status == status.Value);
        q = q.OrderByDescending(p => p.IssuedAt);
        return await PaginatedList<Prescription>.CreateAsync(q, page, pageSize);
    }

    public async Task<Prescription?> GetByIdAsync(Guid id) =>
        await _db.Prescriptions.Include(p => p.Patient).Include(p => p.Doctor).Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Prescription> CreateAsync(Guid patientId, Guid doctorId, Guid? medicalRecordId, DateTime? expiresAt, string? notes, List<(string medication, string dosage, string frequency, int durationDays, int quantity, string? instructions)> items)
    {
        var next = await _db.Prescriptions.CountAsync() + 1;
        var exp = expiresAt ?? DateTime.UtcNow.AddDays(30);
        var rx = new Prescription { PrescriptionNumber = $"RX-{next:D6}", PatientId = patientId, DoctorId = doctorId, MedicalRecordId = medicalRecordId, IssuedAt = DateTime.UtcNow, ExpiresAt = exp, Notes = notes, CreatedAt = DateTime.UtcNow };
        foreach (var (medication, dosage, frequency, durationDays, quantity, instructions) in items)
            rx.Items.Add(new PrescriptionItem { MedicationName = medication, Dosage = dosage, Frequency = frequency, DurationDays = durationDays, Quantity = quantity, Instructions = instructions });
        _db.Prescriptions.Add(rx);
        await _db.SaveChangesAsync();
        return rx;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, PrescriptionStatus status)
    {
        var p = await _db.Prescriptions.FindAsync(id);
        if (p == null) return false;
        p.Status = status; p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
