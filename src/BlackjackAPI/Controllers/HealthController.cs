using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlackjackAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        [Route("")]
        public ActionResult<string> Get()
        {
            return Ok(new
            {
                Status = "Healthy"
            });
        }
    }
}
