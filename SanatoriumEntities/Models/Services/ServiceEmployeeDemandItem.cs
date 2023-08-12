using System;
namespace SanatoriumEntities.Models.Services
{
    public class ServiceEmployeeDemandItem : AbstractServiceDemandItem
    {
        public override string getDatabaseEntityName()
        {
            return "services_employees_demands";
        }
    }
}
