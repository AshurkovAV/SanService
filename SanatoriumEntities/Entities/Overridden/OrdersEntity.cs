using System.Collections.Generic;
using SanatoriumEntities.ServicesClasses;
using SanatoriumEntities.Models.Services;

namespace SanatoriumEntities.Entities.Overriden
{
    public class OrdersEntity : SimpleEntity<Order> 
    {
        public override int delete(int id, bool isHardDelete = false)
        {
            Order order = SEntities.Orders(true).select(id);
            ServiceSessions.GetInstance().releaseOrderSchedule(order);
            
            return base.delete(id, isHardDelete);
        }

        public override int deleteList(string filter, bool isHardDelete = false)
        {
            int res = 0;

            List<Order> items = SEntities.Orders(true).selectList(filter, "id");

            foreach (Order item in items)
            {
                try {
                    ServiceSessions.GetInstance().releaseOrderSchedule(item);
                    base.delete(item.id ?? 0, isHardDelete);
                    res += 1;
                } catch (System.Exception e) {
                    Log.Get().put(Log.ERR, e.Message);
                }
            }

            return res;
        }
    }
}
