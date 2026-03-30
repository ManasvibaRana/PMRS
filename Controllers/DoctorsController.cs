using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMRS.Services;

namespace PMRS.Controllers;

[Authorize(Roles = "Admin")]
public class DoctorsController : Controller
{
    private readonly DoctorService _doctorService;

    public DoctorsController(DoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> Index() => View(await _doctorService.GetListAsync());

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string firstName, string lastName, string specialisation, string licenseNumber, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(specialisation) || string.IsNullOrWhiteSpace(licenseNumber) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
        {
            TempData["Error"] = "All fields required.";
            return View();
        }
        await _doctorService.CreateAsync(firstName, lastName, specialisation, licenseNumber, email, phone);
        TempData["Success"] = "Doctor added.";
        return RedirectToAction(nameof(Index));
    }
}
