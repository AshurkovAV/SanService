using Microsoft.AspNetCore.Mvc;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.Services;
using System;
using System.Collections.Generic;

namespace SanService.Controllers.Services
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private SimpleEntity<Order> _orderResources = new SimpleEntity<Order>();

        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return _orderResources.selectList("").ToArray();
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult GetById(string id)
        {
            try
            {
                var idint = Convert.ToInt32(id);
                var item = _orderResources.select(idint);
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
    }
}
