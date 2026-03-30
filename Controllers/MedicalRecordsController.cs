using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMRS.Entities;
using PMRS.Enums;
using PMRS.Services;

namespace PMRS.Controllers;

[Authorize]
public class MedicalRecordsController : Controller
{
    private readonly MedicalRecordService _recordService;
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;

    public MedicalRecordsController(MedicalRecordService recordService, PatientService patientService, DoctorService doctorService)
    {
        _recordService = recordService;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index(Guid? patientId, int pageNumber = 1) => View(await _recordService.GetListAsync(patientId, pageNumber, 10));

    public async Task<IActionResult> Details(Guid id)
    {
        var rec = await _recordService.GetByIdAsync(id);
        if (rec == null) return NotFound();
        return View(rec);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Patients = await _patientService.GetListAsync(null, true, 1, 500);
        ViewBag.Doctors = await _doctorService.GetListAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid patientId, Guid doctorId, Guid? appointmentId, RecordType recordType, string title, string content, string? diagnosis, string? treatment)
    {
        await _recordService.CreateAsync(patientId, doctorId, appointmentId, recordType, title, content ?? "", diagnosis, treatment);
        TempData["Success"] = "Record created.";
        return RedirectToAction(nameof(Index));
    }
}
