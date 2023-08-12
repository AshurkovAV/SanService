using Microsoft.AspNetCore.Mvc;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using SanService.Services;

namespace SanService.Controllers.Services
{
    [ApiController]
    [Route("[controller]")]
    public class CancellationProceduresController : ControllerBase
    {
        private SimpleEntity<CancellationProcedures> _simpleEntity = new SimpleEntity<CancellationProcedures>();
        private SimpleEntity<Order> _orderResources = new SimpleEntity<Order>();
        private UserService _userService = new UserService();

        [HttpPost("add")]
        public IActionResult Add(CancellationProcedures cancellationProcedures)
        {
            try
            {
                var claimsIndentity = this.HttpContext.User.Identity as ClaimsIdentity;
                var emplId = claimsIndentity.Claims.FirstOrDefault(x =>
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (cancellationProcedures.order_id != null && cancellationProcedures.service_id == null)
                {
                    var orderResult = _orderResources.@select((int)cancellationProcedures.order_id);

                    cancellationProcedures.service_id = orderResult?.service_id;
                }
                
                int? completedEmployeeId = null;
                if (emplId != null)
                {
                    completedEmployeeId = Convert.ToInt32(emplId.Value);
                    completedEmployeeId = _userService.GetEmpGenToid((int)completedEmployeeId).Data.id;
                }

                cancellationProcedures.completed_employee_id = completedEmployeeId;

                var item = _simpleEntity.insertAndSelect(cancellationProcedures);
                if (item == null || item.id == null)
                {
                    return BadRequest();
                }
                return new ObjectResult(item);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        [HttpGet("{id}", Name = "GetCancellationProcedures")]
        public IActionResult GetToId(string id)
        {
            try
            {
                var idint = Convert.ToInt32(id);
                var item = _simpleEntity.select(idint);
                if (item == null || item.id == null)
                {
                    return NotFound();
                }
                return new ObjectResult(item);
            }
            catch (Exception e)
            {
                return NotFound();
            }

        }

        [HttpGet, Route("data")]
        public IActionResult GetToEmpId(int empid, DateTime dt)
        {
            try
            {
                var item = _simpleEntity.selectList($"schedule_date BETWEEN '{dt.ToString("yyyyMMdd 00:00")}' AND '{dt.ToString("yyyyMMdd 23:59")}' and completed_employee_id = {empid}","id");
                if (item == null)
                {
                    return NotFound();
                }
                return new ObjectResult(item);
            }
            catch (Exception e)
            {
                return NotFound();
            }

        }
    }
}
