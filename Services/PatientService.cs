using Microsoft.EntityFrameworkCore;
using PMRS.Data;
using PMRS.Entities;
using PMRS.Enums;

namespace PMRS.Services;

public class PatientService
{
    private readonly PMRSDbContext _db;

    public PatientService(PMRSDbContext db) => _db = db;

    public async Task<PaginatedList<Patient>> GetListAsync(string? search, bool? isActive, int page = 1, int pageSize = 10)
    {
        var q = _db.Patients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => (p.FirstName + " " + p.LastName + " " + p.PatientNumber + " " + p.Email).Contains(search));
        if (isActive.HasValue) q = q.Where(p => p.IsActive == isActive.Value);
        q = q.OrderByDescending(p => p.RegisteredAt);
        return await PaginatedList<Patient>.CreateAsync(q, page, pageSize);
    }

    public async Task<Patient?> GetByIdAsync(Guid id) =>
        await _db.Patients.Include(p => p.Address).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Patient> CreateAsync(string firstName, string lastName, DateTime dob, Gender gender, string email, string phone, string? bloodType, string? allergies, string? chronicConditions, string? emergencyName, string? emergencyPhone, Address address)
    {
        var next = await _db.Patients.CountAsync() + 1;
        var patient = new Patient
        {
            PatientNumber = $"PAT-{next:D6}", FirstName = firstName, LastName = lastName, DateOfBirth = dob, Gender = gender, Email = email, Phone = phone,
            BloodType = bloodType, Allergies = allergies, ChronicConditions = chronicConditions, EmergencyContactName = emergencyName, EmergencyContactPhone = emergencyPhone,
            Address = address, CreatedAt = DateTime.UtcNow, RegisteredAt = DateTime.UtcNow
        };
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        return patient;
    }

    public async Task<bool> UpdateAsync(Guid id, string firstName, string lastName, DateTime dob, Gender gender, string email, string phone, string? bloodType, string? allergies, string? chronicConditions, string? emergencyName, string? emergencyPhone, Address address, bool isActive)
    {
        var p = await _db.Patients.FindAsync(id);
        if (p == null) return false;
        p.FirstName = firstName; p.LastName = lastName; p.DateOfBirth = dob; p.Gender = gender; p.Email = email; p.Phone = phone;
        p.BloodType = bloodType; p.Allergies = allergies; p.ChronicConditions = chronicConditions; p.EmergencyContactName = emergencyName; p.EmergencyContactPhone = emergencyPhone;
        p.Address = address; p.IsActive = isActive; p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var p = await _db.Patients.FindAsync(id);
        if (p == null) return false;
        p.IsActive = false; p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
