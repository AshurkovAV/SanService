using Microsoft.AspNetCore.Mvc;
using SanService.Infrastructure.Filters;
using System;


namespace SanService.Controllers.test
{

    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [SimpleResourceFilter]
       
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("Login")]
        [SimpleResourceFilter]
        public IActionResult Get()
        {
            try
            {

                return new ObjectResult(null);
            }
            catch (Exception e)
            {
                return NotFound();
            }

        }
    }
}