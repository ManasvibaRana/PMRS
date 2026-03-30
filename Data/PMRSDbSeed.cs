using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PMRS.Entities;
using PMRS.Enums;

namespace PMRS.Data;

public static class PMRSDbSeed
{
    public static async Task SeedAsync(PMRSDbContext ctx, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { "Admin", "Doctor", "Receptionist" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        if (!await userManager.Users.AnyAsync())
        {
            var admin = new ApplicationUser { UserName = "admin@pmrs.com", Email = "admin@pmrs.com", FirstName = "Admin", LastName = "User", EmailConfirmed = true };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
            var rec = new ApplicationUser { UserName = "reception@pmrs.com", Email = "reception@pmrs.com", FirstName = "Reception", LastName = "User", EmailConfirmed = true };
            await userManager.CreateAsync(rec, "Reception@123");
            await userManager.AddToRoleAsync(rec, "Receptionist");
        }

        if (!await ctx.Doctors.AnyAsync())
        {
            ctx.Doctors.AddRange(
                new Doctor { FirstName = "Emily", LastName = "Carter", Specialisation = "General Practice", LicenseNumber = "GP-001", Email = "emily@pmrs.com", Phone = "555-0101", CreatedAt = DateTime.UtcNow },
                new Doctor { FirstName = "James", LastName = "Wilson", Specialisation = "Cardiology", LicenseNumber = "CARD-001", Email = "james@pmrs.com", Phone = "555-0102", CreatedAt = DateTime.UtcNow });
            await ctx.SaveChangesAsync();
        }

        if (!await ctx.Patients.AnyAsync())
        {
            ctx.Patients.AddRange(
                new Patient { PatientNumber = "PAT-000001", FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1985, 6, 15), Gender = Gender.Male, BloodType = "O+", Email = "john@example.com", Phone = "555-1001", Address = new Address { Street = "123 Main St", City = "Springfield", State = "IL", PostalCode = "62701", Country = "USA" }, CreatedAt = DateTime.UtcNow, RegisteredAt = DateTime.UtcNow },
                new Patient { PatientNumber = "PAT-000002", FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1990, 3, 22), Gender = Gender.Female, BloodType = "A+", Email = "jane@example.com", Phone = "555-1002", Address = new Address { Street = "456 Oak Ave", City = "Springfield", State = "IL", PostalCode = "62702", Country = "USA" }, CreatedAt = DateTime.UtcNow, RegisteredAt = DateTime.UtcNow });
            await ctx.SaveChangesAsync();
        }
    }
}
