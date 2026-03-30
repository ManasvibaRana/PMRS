using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMRS.Entities;
using PMRS.Enums;
using PMRS.Services;

namespace PMRS.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly AppointmentService _appointmentService;
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;

    public AppointmentsController(AppointmentService appointmentService, PatientService patientService, DoctorService doctorService)
    {
        _appointmentService = appointmentService;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index(int pageNumber = 1) => View(await _appointmentService.GetListAsync(null, null, pageNumber, 10));

    public async Task<IActionResult> Details(Guid id)
    {
        var apt = await _appointmentService.GetByIdAsync(id);
        if (apt == null) return NotFound();
        return View(apt);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Patients = await _patientService.GetListAsync(null, true, 1, 500);
        ViewBag.Doctors = await _doctorService.GetListAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid patientId, Guid doctorId, DateTime scheduledAt, int durationMinutes, string chiefComplaint, string? notes)
    {
        await _appointmentService.CreateAsync(patientId, doctorId, scheduledAt, durationMinutes, chiefComplaint ?? "", notes);
        TempData["Success"] = "Appointment scheduled.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var apt = await _appointmentService.GetByIdAsync(id);
        if (apt == null) return NotFound();
        return View(apt);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DateTime scheduledAt, int durationMinutes, string chiefComplaint, string? notes, AppointmentStatus status)
    {
        if (!await _appointmentService.UpdateAsync(id, scheduledAt, durationMinutes, chiefComplaint ?? "", notes, status)) return NotFound();
        TempData["Success"] = "Appointment updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? cancellationReason)
    {
        if (!await _appointmentService.CancelAsync(id, cancellationReason)) return NotFound();
        TempData["Success"] = "Appointment cancelled.";
        return RedirectToAction(nameof(Index));
    }
}
