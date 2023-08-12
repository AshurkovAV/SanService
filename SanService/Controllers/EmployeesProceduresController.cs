using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Models.View;
using SanatoriumEntities.ServicesClasses;
using SanService.Services;

namespace SanService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesProceduresController : ControllerBase
    {
        private SimpleEntity<EmployeesProceduresList> _simpleEntity = new SimpleEntity<EmployeesProceduresList>();
        UserService _userservice = new UserService();

        [HttpGet, Route("data")]
        public IActionResult GetRegisterExecuted(int id, int empid, DateTime dt)
        {
            try
            {
                var claimsIndentity = this.HttpContext.User.Identity as ClaimsIdentity;
                var emplId = claimsIndentity.Claims.FirstOrDefault(x =>
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                int ? completedEmployeeId = null;
                if (emplId != null)
                {
                    completedEmployeeId = Convert.ToInt32(emplId.Value);
                }
                var orderResult =
                    _simpleEntity.selectList(
                        $"dt BETWEEN '{dt.ToString("yyyyMMdd 00:00")}' AND '{dt.ToString("yyyyMMdd 23:59")}' and csession_id={id} and employee_general_id = {empid}", "start_time");

                if (orderResult == null)
                {
                    return NoContent();
                }
                return new ObjectResult(orderResult);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet, Route("data/session")]
        public IActionResult GetRegisterExecutedToSession(int id, int empid)
        {
            try
            {
                var claimsIndentity = this.HttpContext.User.Identity as ClaimsIdentity;
                var emplId = claimsIndentity.Claims.FirstOrDefault(x =>
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                int? completedEmployeeId = null;
                if (emplId != null)
                {
                    completedEmployeeId = Convert.ToInt32(emplId.Value);

                    var orderResult =
                        _simpleEntity.selectList(
                            $"employee_general_id = {empid} and csession_id={id}", "start_time");

                    if (orderResult == null)
                    {
                        return NoContent();
                    }

                    return new ObjectResult(orderResult);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet, Route("data/order")]
        public IActionResult GetRegisterExecutedToOrder(int id, int empid)
        {
            try
            {
                var claimsIndentity = this.HttpContext.User.Identity as ClaimsIdentity;
                var emplId = claimsIndentity.Claims.FirstOrDefault(x =>
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                int? completedEmployeeId = null;
                if (emplId != null)
                {
                    completedEmployeeId = Convert.ToInt32(emplId.Value);

                    var orderResult =
                        _simpleEntity.selectList(
                            $"employee_general_id = {empid} and order_id={id}", "start_time");

                    if (orderResult == null)
                    {
                        return NoContent();
                    }

                    return new ObjectResult(orderResult);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet, Route("data/id")]
        public IActionResult GetRegisterExecutedToId(int id)
        {
            try
            {
                var orderResult = _simpleEntity.select(id);

                if (orderResult.id == null)
                {
                    return NoContent();
                }
                return new ObjectResult(orderResult);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}
