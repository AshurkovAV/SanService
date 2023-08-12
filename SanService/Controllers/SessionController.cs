using Microsoft.Extensions.Configuration;

namespace SanService.Controllers
{
    public class SessionController
    {
        private readonly IConfiguration _config;

        public SessionController(IConfiguration config)
        {
            _config = config;
        }
    }
}
