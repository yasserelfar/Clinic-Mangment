using System.Security.Claims;
using ClinicManagement.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Models.User> _passwordHasher;

    public AccountController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Models.User>();
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // 1. Find user by email
        var user = _context.Users
            .FirstOrDefault(u => u.Email == email);

        // 2. If user doesn't exist
        if (user == null)
        {
            ModelState.AddModelError(
                "",
                "Invalid email or password."
            );

            return View();
        }
        if (!user.IsActive) 
        {
            ModelState.AddModelError("", "Your account is inactive."); 
            return View();
        }
        // 3. Verify password
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        );

        // 4. If password is incorrect
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                "",
                "Invalid email or password."
            );

            return View();
        }

        // 5. Create user's claims
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role
            )
        };

        // 6. Create identity
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        // 7. Create principal
        var principal = new ClaimsPrincipal(identity);

        // 8. Create authentication cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        // 9. Redirect according to role
        if (user.Role == "Doctor")
        {
            return RedirectToAction(
                "Dashboard",
                "Doctors"
            );
        }

        if (user.Role == "Reception")
        {
            return RedirectToAction(
                "Dashboard",
                "Reception"
            );
        }

        if (user.Role == "Admin")
        {
            return RedirectToAction(
                "Dashboard",
                "Admin"
            );
        }

        return RedirectToAction(
            "Index",
            "Home"
        );
    }

    // POST: /Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction(
            "Login",
            "Account"
        );
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}