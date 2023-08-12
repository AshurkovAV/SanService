using System.Collections.Generic;
using SanatoriumEntities.Models.Session;
using SanatoriumEntities.Models.Services;

namespace SanatoriumEntities.Interfaces.Services
{
    public interface IOrders
    {
        List<Order> generateOrdersList(ProgramAppointment programAppointment, bool updateDatabase = false);
    }
}
