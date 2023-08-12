using System;
using SanatoriumEntities.ServicesClasses;

namespace SanatoriumEntities.Models.Services
{
    public class ResourceSlot : BaseModel
    {
        public int       resource_category_id   { get; set; }
        public int       resource_general_id    { get; set; }
        public int       duration_min           { get; set; } = ServiceSlots.SLOT_DURATION_MINUTES;
        public DateTime  slot_start_time        { get; set; }
        public DateTime  slot_end_time          { get; set; }
        public int       slot_ordinal           { get; set; } = 1;
        public int?      order_id               { get; set; }
        public int?      service_id             { get; set; }
        public int?      service_category_id    { get; set; }
        public int?      customer_id            { get; set; }
        public int?      iteration              { get; set; }

        public override string getDatabaseEntityName()
        {
            return "resources_slots";
        }
    }
}
