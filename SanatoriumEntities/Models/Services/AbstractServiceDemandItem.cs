using System;
namespace SanatoriumEntities.Models.Services
{
    public abstract class AbstractServiceDemandItem : BaseModel
    {
        public int          service_id                  { get; set; }
        public int?         resource_general_id         { get; set; }
        public int          resource_iteration_level    { get; set; } = 1;
        public int?         alt_group_id                { get; set; }
        public DateTime?    last_usage                  { get; set; }
    }
}
