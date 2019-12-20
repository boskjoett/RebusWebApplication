using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Zylinc.Common.MessageBus.Messages;
using Zylinc.Common.MessageBus.Messages.RequestResponses.Organization;

namespace RebusWebApplication
{
    public class UserLoginRequestModel : PageModel
    {
        private readonly IBus _messageBus;
        private readonly ILogger<UserLoginRequestModel> _logger;

        [BindProperty]
        public string Email { get; set; }

        public UserLoginRequestModel(IBus messageBus, ILogger<UserLoginRequestModel> logger)
        {
            _messageBus = messageBus;
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

            Guid id = Guid.NewGuid();
            UserLoginRequest message = new UserLoginRequest(id, Startup.InputQueueName, Email, "eruer7453tbu");

            try
            {
                _logger.LogInformation($"Publishing UserLoginRequest message. ID: {id}, Email: {Email}");
                await _messageBus.Publish(message, RebusConfiguration.Headers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception publishing UserLoginRequest message: {ex.Message}");
            }

            return RedirectToPage("./Index");
        }
    }
}