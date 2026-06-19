using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace email_attachment_orders.Services
{
    public class EmailService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _mailbox;

        public EmailService(GraphServiceClient graphAuthService)
        {
            _graphClient = graphAuthService;
            _mailbox = Environment.GetEnvironmentVariable()
        }
    }
}