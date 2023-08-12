using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanatoriumCore.Secure;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.Services;
using SanService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SanService.Infrastructure.Filters;

namespace SanService.Controllers.Services
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        public EmployeesController()
        {            
        }
       
        private SimpleEntity<EmployeeResource> _employeesResources = new SimpleEntity<EmployeeResource>();


        [HttpGet]
        public IEnumerable<EmployeeResource> Get()
        {            
            return _employeesResources.selectList("").ToArray();            
        }

        [HttpGet("{id}", Name = "GetEmployees")]
        public IActionResult GetById(string id)
        {
            try
            {
                var idint = Convert.ToInt32(id);
                var item = _employeesResources.select(idint);
                if (item == null || item.id == null)
                {
                    return NotFound();
                }
                return new ObjectResult(item);
            }
            catch(Exception e) {
                return NotFound();
            }           
            
        }
    }
}
