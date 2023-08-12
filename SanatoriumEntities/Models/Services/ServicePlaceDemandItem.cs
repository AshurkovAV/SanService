using System;
namespace SanatoriumEntities.Models.Services
{
    public class ServicePlaceDemandItem : AbstractServiceDemandItem
    {
        public override string getDatabaseEntityName()
        {
            return "services_places_demands";
        }
    }
}
