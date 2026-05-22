using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Списък на потребителите
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var model = new List<AppUser>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            model.Add(new AppUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault()
            });
        }

        return View(model);
    }

    // CREATE
    public IActionResult Create()
    {
        ViewBag.Roles = _roleManager.Roles.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RegisterViewModel model, string role)
    {
        ViewBag.Roles = _roleManager.Roles.ToList();

        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    // EDIT (GET)
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = _roleManager.Roles.ToList();

        var model = new AppUser
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Role = roles.FirstOrDefault()
        };

        return View(model);
    }

    // EDIT (POST)
    [HttpPost]
    public async Task<IActionResult> Edit(
        string id,
        string username,
        string email,
        string role,
        string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.UserName = username;
        user.Email = email;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError("", error.Description);

            ViewBag.Roles = _roleManager.Roles.ToList();

            return View(new AppUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = role
            });
        }

        // Password change
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var passwordValidator =
                HttpContext.RequestServices.GetRequiredService<IPasswordValidator<ApplicationUser>>();

            var passwordHasher =
                HttpContext.RequestServices.GetRequiredService<IPasswordHasher<ApplicationUser>>();

            var result = await passwordValidator.ValidateAsync(_userManager, user, newPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("newPassword", error.Description);

                ViewBag.Roles = _roleManager.Roles.ToList();

                return View(new AppUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = role
                });
            }

            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            await _userManager.UpdateAsync(user);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction(nameof(Index));
    }

    // DELETE (GET)
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return View(new AppUser
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Role = roles.FirstOrDefault()
        });
    }

    // DELETE (POST)
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (user.Email == "atanas.beshovishki@gmail.com")
        {
            ModelState.AddModelError("", "❌ Главният администратор не може да бъде изтрит.");
            return View("Delete", new AppUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            });
        }

        await _userManager.DeleteAsync(user);

        TempData["SuccessMessage"] = "✅ Потребителят беше изтрит.";
        return RedirectToAction(nameof(Index));
    }
}