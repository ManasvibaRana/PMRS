using PMRS.Enums;

namespace PMRS.Entities;

public class MedicalRecord : AuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public RecordType RecordType { get; set; } = RecordType.Consultation;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
