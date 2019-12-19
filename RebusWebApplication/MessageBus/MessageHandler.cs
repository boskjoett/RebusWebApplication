using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Zylinc.Common.MessageBus.Messages.RequestResponses.Organization;

namespace RebusWebApplication.MessageBus
{
    public class MessageHandler :
        IHandleMessages<UserLoginResponse>
    {
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(ILogger<MessageHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserLoginResponse message)
        {
            _logger.LogInformation("UserLoginResponse message received");
            return Task.CompletedTask;
        }
    }
}
