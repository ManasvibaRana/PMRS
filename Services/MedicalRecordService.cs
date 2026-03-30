using Microsoft.EntityFrameworkCore;
using PMRS.Data;
using PMRS.Entities;
using PMRS.Enums;

namespace PMRS.Services;

public class MedicalRecordService
{
    private readonly PMRSDbContext _db;

    public MedicalRecordService(PMRSDbContext db) => _db = db;

    public async Task<PaginatedList<MedicalRecord>> GetListAsync(Guid? patientId, int page = 1, int pageSize = 10)
    {
        IQueryable<MedicalRecord> q = _db.MedicalRecords.AsNoTracking().Include(m => m.Patient).Include(m => m.Doctor);
        if (patientId.HasValue) q = q.Where(m => m.PatientId == patientId.Value);
        q = q.OrderByDescending(m => m.RecordedAt);
        return await PaginatedList<MedicalRecord>.CreateAsync(q, page, pageSize);
    }

    public async Task<MedicalRecord?> GetByIdAsync(Guid id) =>
        await _db.MedicalRecords.Include(m => m.Patient).Include(m => m.Doctor).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<MedicalRecord> CreateAsync(Guid patientId, Guid doctorId, Guid? appointmentId, RecordType recordType, string title, string content, string? diagnosis, string? treatment)
    {
        var rec = new MedicalRecord { PatientId = patientId, DoctorId = doctorId, AppointmentId = appointmentId, RecordType = recordType, Title = title, Content = content ?? "", Diagnosis = diagnosis, Treatment = treatment, RecordedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };
        _db.MedicalRecords.Add(rec);
        await _db.SaveChangesAsync();
        return rec;
    }
}
