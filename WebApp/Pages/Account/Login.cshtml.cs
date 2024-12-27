using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Credential.UserName == "admin" && Credential.Password == "password")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "hdung72267@gmail.com"),
                    new Claim("Manager", "true"),
                    new Claim("Department", "HR"),
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
