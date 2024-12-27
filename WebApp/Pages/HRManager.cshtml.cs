using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
