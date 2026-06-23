using Microsoft.AspNetCore.Mvc;
using email_attachment_orders.Services;

namespace email_attachment_orders.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly OutletMappingService _outletMapService;

        public EmailController(EmailService emailService, OutletMappingService outletMapService)
        {
            _emailService = emailService;
            _outletMapService = outletMapService;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _emailService.GetParsedEmailOrdersForTestingAsync();
                var email = "vikas@gmail.com";
                var response = await _outletMapService.GetMappingByEmailAsync(email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }
    }
}