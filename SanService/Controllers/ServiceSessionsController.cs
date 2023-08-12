using Microsoft.AspNetCore.Mvc;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.ServicesClasses;
using System;
using System.Linq;
using System.Security.Claims;

namespace SanService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceSessionsController : ControllerBase
    {
        private SimpleEntity<Order> _orderResources = new SimpleEntity<Order>();

        [HttpGet("registerexecuted/{order}/{iteration}")]
        public IActionResult GetRegisterExecuted(int order, int iteration)
        {
            try
            {
                var claimsIndentity = this.HttpContext.User.Identity as ClaimsIdentity;
                var emplId = claimsIndentity.Claims.FirstOrDefault(x =>
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var orderResult = _orderResources.@select(order);
                int? completedEmployeeId = null;
                if (emplId != null)
                {
                    completedEmployeeId = Convert.ToInt32(emplId.Value);
                }

                if (orderResult == null || orderResult.id == 0 || orderResult.id == null)
                {
                    return NoContent();
                }
                return new ObjectResult(ServiceSessions.GetInstance()
                    .registerExecutedOrderIteration(orderResult, completedEmployeeId, iteration));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("unregisterexecuted/{order}/{iteration}")]
        public IActionResult GetUnregisterExecuted(int order, int iteration)
        {
            try
            {
                var orderResult = _orderResources.@select(order);
                if (orderResult == null || orderResult.id == 0 || orderResult.id == null)
                {
                    return NoContent();
                }
                return new ObjectResult(ServiceSessions.GetInstance()
                    .unregisterExecutedOrderIteration(orderResult, iteration));
            }
            catch (Exception e)
            {
                return NotFound();
            }

        }
    }
}
