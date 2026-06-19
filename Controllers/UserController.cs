using Microsoft.AspNetCore.Mvc;
using email_attachment_orders.Services;

namespace email_attachment_orders.Controllers
{
    [ApiController]
    [Route("api")]

    public class UserController : ControllerBase
    {
        private readonly GraphAuthService _graphAuthService;

        public UserController(GraphAuthService graphAuthService)
        {
            _graphAuthService = graphAuthService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var graphClient = _graphAuthService.getClient();
            string mailbox = Environment.GetEnvironmentVariable("MAILBOX_ADDRESS") ?? throw new Exception("Mailbox Address not found");

            var user = await graphClient.Users[mailbox].GetAsync();
            return Ok(new
            {
                Name = user?.DisplayName,
                Mail = user?.Mail,
                PrincipalName = user?.UserPrincipalName
            });
        }
    }
}