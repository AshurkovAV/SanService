using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
