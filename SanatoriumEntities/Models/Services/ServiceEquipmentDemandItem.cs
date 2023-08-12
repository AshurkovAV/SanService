using System;
namespace SanatoriumEntities.Models.Services
{
    public class ServiceEquipmentDemandItem : AbstractServiceDemandItem
    {
        public override string getDatabaseEntityName()
        {
            return "services_equipment_demands";
        }
    }
}
