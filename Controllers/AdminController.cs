using EventPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
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

    // Създаване на потребител
    public IActionResult Create()
    {
        ViewBag.Roles = _roleManager.Roles.ToList(); // задаваме ролите за DropDown
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RegisterViewModel model, string role)
    {
        ViewBag.Roles = _roleManager.Roles.ToList();

        if (!ModelState.IsValid)
            return View(model);

        var user = new IdentityUser
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

    // Edit
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

    [HttpPost]
    public async Task<IActionResult> Edit(string id, string username, string email, string role, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // 🆕 Позволяваме промяна и на потребителското име
        user.UserName = username;
        user.Email = email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError("", error.Description);
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(new AppUser { Id = user.Id, Email = user.Email, UserName = user.UserName, Role = role });
        }

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var passwordValidator = HttpContext.RequestServices.GetRequiredService<IPasswordValidator<IdentityUser>>();
            var passwordHasher = HttpContext.RequestServices.GetRequiredService<IPasswordHasher<IdentityUser>>();
            var result = await passwordValidator.ValidateAsync(_userManager, user, newPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("newPassword", error.Description);

                ViewBag.Roles = _roleManager.Roles.ToList();
                var model = new AppUser { Id = user.Id, Email = user.Email, UserName = user.UserName, Role = role };
                return View(model);
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


    // Delete

    // GET: Admin/Delete
    
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var model = new AppUser
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Role = roles.FirstOrDefault()
        };

        return View(model);
    }


    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // 🧱 Проверка дали е главният администратор
        if (user.Email == "atanas.beshovishki@gmail.com")
        {
            ModelState.AddModelError("", "❌ Главният администратор не може да бъде изтрит.");
            var roles = await _userManager.GetRolesAsync(user);
            var model = new AppUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault()
            };
            return View("Delete", model);
        }

        // 🧠 Проверка дали е последният останал админ
        var rolesForUser = await _userManager.GetRolesAsync(user);
        if (rolesForUser.Contains("Admin"))
        {
            var allAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
            if (allAdmins <= 1)
            {
                ModelState.AddModelError("", "⚠️ Не може да изтриете последния администратор в системата.");
                var model = new AppUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = rolesForUser.FirstOrDefault()
                };
                return View("Delete", model);
            }
        }

        await _userManager.DeleteAsync(user);
        TempData["SuccessMessage"] = "✅ Потребителят беше успешно изтрит.";
        return RedirectToAction(nameof(Index));
    }


}

