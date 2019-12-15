using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Zylinc.Common.MessageBus.Messages.RequestResponses.Organization;

namespace RebusWebApplication
{
    public class UserLoginRequestModel : PageModel
    {
        private readonly ILogger<UserLoginRequestModel> _logger;

        [BindProperty]
        public string Email { get; set; }

        public UserLoginRequestModel(ILogger<UserLoginRequestModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

//            UserLoginRequest message = new UserLoginRequest(Guid.NewGuid(), )

            return RedirectToPage("./Index");
        }
    }
}