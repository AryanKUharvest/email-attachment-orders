using Azure.Identity;
using Microsoft.Graph;

namespace email_attachment_orders.Services
{
    public class GraphAuthService
    {
        private readonly GraphServiceClient _graphClient;
        
        public GraphAuthService()
        {
            string tenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? throw new Exception("Tenant ID not found");
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID") ?? throw new Exception("Client ID not found");
            string clientAuthSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET_VALUE") ?? throw new Exception("Client Secret not found");

            var credential = new ClientSecretCredential(tenantId, clientId, clientAuthSecret);
            _graphClient = new GraphServiceClient(
                    credential,
                    new[] {"https://graph.microsoft.com/.default"}
                );
        }

        public GraphServiceClient getClient()
        {
            return _graphClient;
        }
    }
}