using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AliasGame.Controllers
{
    [ApiController]
    [Route("dev")]
    public class DevController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public DevController(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var content = System.IO.File.ReadAllText(_env.WebRootPath + "/templates/game.html");
            return Ok(content);
        }
    }
}