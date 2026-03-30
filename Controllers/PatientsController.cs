using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMRS.Entities;
using PMRS.Services;
using PMRS.ViewModels;

namespace PMRS.Controllers;

[Authorize]
public class PatientsController : Controller
{
    private readonly PatientService _patientService;

    public PatientsController(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> Index(string? searchTerm, bool? isActive, int pageNumber = 1)
    {
        ViewBag.SearchTerm = searchTerm;
        ViewBag.IsActive = isActive;
        return View(await _patientService.GetListAsync(searchTerm, isActive, pageNumber, 10));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null) return NotFound();
        return View(patient);
    }

    public IActionResult Create() => View(new PatientFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PatientFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var address = new Address { Street = model.Street ?? "", City = model.City ?? "", State = model.State ?? "", PostalCode = model.PostalCode ?? "", Country = model.Country ?? "" };
        var patient = await _patientService.CreateAsync(model.FirstName, model.LastName, model.DateOfBirth, model.Gender, model.Email, model.Phone, model.BloodType, model.Allergies, model.ChronicConditions, model.EmergencyContactName, model.EmergencyContactPhone, address);
        TempData["Success"] = "Patient registered.";
        return RedirectToAction(nameof(Details), new { id = patient.Id });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var p = await _patientService.GetByIdAsync(id);
        if (p == null) return NotFound();
        return View(new PatientFormViewModel
        {
            Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, DateOfBirth = p.DateOfBirth, Gender = p.Gender,
            Email = p.Email, Phone = p.Phone, BloodType = p.BloodType, Allergies = p.Allergies, ChronicConditions = p.ChronicConditions,
            EmergencyContactName = p.EmergencyContactName, EmergencyContactPhone = p.EmergencyContactPhone,
            Street = p.Address.Street, City = p.Address.City, State = p.Address.State, PostalCode = p.Address.PostalCode, Country = p.Address.Country,
            IsActive = p.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PatientFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var address = new Address { Street = model.Street ?? "", City = model.City ?? "", State = model.State ?? "", PostalCode = model.PostalCode ?? "", Country = model.Country ?? "" };
        if (!await _patientService.UpdateAsync(id, model.FirstName, model.LastName, model.DateOfBirth, model.Gender, model.Email, model.Phone, model.BloodType, model.Allergies, model.ChronicConditions, model.EmergencyContactName, model.EmergencyContactPhone, address, model.IsActive))
            return NotFound();
        TempData["Success"] = "Patient updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _patientService.DeleteAsync(id);
        TempData["Success"] = "Patient deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
