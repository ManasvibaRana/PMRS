using Microsoft.EntityFrameworkCore;
using PMRS.Data;
using PMRS.Entities;

namespace PMRS.Services;

public class DoctorService
{
    private readonly PMRSDbContext _db;

    public DoctorService(PMRSDbContext db) => _db = db;

    public async Task<List<Doctor>> GetListAsync() =>
        await _db.Doctors.AsNoTracking().Where(d => d.IsActive).OrderBy(d => d.LastName).ToListAsync();

    public async Task<Doctor?> GetByIdAsync(Guid id) => await _db.Doctors.FindAsync(id);

    public async Task<Doctor> CreateAsync(string firstName, string lastName, string specialisation, string licenseNumber, string email, string phone)
    {
        var doctor = new Doctor
        {
            FirstName = firstName, LastName = lastName, Specialisation = specialisation, LicenseNumber = licenseNumber,
            Email = email, Phone = phone, CreatedAt = DateTime.UtcNow
        };
        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();
        return doctor;
    }
}
