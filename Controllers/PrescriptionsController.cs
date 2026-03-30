using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMRS.Entities;
using PMRS.Enums;
using PMRS.Services;

namespace PMRS.Controllers;

[Authorize]
public class PrescriptionsController : Controller
{
    private readonly PrescriptionService _prescriptionService;
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;

    public PrescriptionsController(PrescriptionService prescriptionService, PatientService patientService, DoctorService doctorService)
    {
        _prescriptionService = prescriptionService;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index(Guid? patientId, PrescriptionStatus? status, int pageNumber = 1) =>
        View(await _prescriptionService.GetListAsync(patientId, status, pageNumber, 10));

    public async Task<IActionResult> Details(Guid id)
    {
        var rx = await _prescriptionService.GetByIdAsync(id);
        if (rx == null) return NotFound();
        return View(rx);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Patients = await _patientService.GetListAsync(null, true, 1, 500);
        ViewBag.Doctors = await _doctorService.GetListAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid patientId, Guid doctorId, Guid? medicalRecordId, DateTime? expiresAt, string? notes, string medicationName, string dosage, string frequency, int durationDays, int quantity, string? instructions)
    {
        var items = new List<(string, string, string, int, int, string?)> { (medicationName, dosage, frequency, durationDays, quantity, instructions) };
        await _prescriptionService.CreateAsync(patientId, doctorId, medicalRecordId, expiresAt, notes, items);
        TempData["Success"] = "Prescription created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, PrescriptionStatus status)
    {
        if (!await _prescriptionService.UpdateStatusAsync(id, status)) return NotFound();
        TempData["Success"] = "Status updated.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
