using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AliasGame.Controllers
{
    public class EntryController : Controller
    {
        private readonly IWebHostEnvironment _env;
        
        public EntryController(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var content = System.IO.File.ReadAllText(_env.WebRootPath + "/templates/index.html");
            return base.Content(content, "text/html");
        }
        
        [HttpGet("fetch_game_html")]
        public IActionResult FetchGameHtml()
        {
            var content = System.IO.File.ReadAllText(_env.WebRootPath + "/templates/game.html");
            return Ok(content);
        }
    }
}