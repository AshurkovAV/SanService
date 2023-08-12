using SanatoriumEntities.Models;
using SanatoriumEntities.Models.Program;
using SanatoriumEntities.Models.Session;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Entities.Overriden;
using SanatoriumEntities.Entities.Aggregates;
using SanatoriumEntities.Interfaces;
using CustomerAggregate = SanatoriumEntities.Models.Customer.CustomerAggregate;

namespace SanatoriumEntities.Entities
{
    public class EntityBuilder
    {
        public static ISanatoriumSimpleEntity<ModelType> getSimpleEntity<ModelType>() where ModelType: BaseModel, new()
        {
            if (typeof(ModelType) == typeof(Program))
            {
                return (ISanatoriumSimpleEntity<ModelType>) new ProgramsEntity();
            }

            if (typeof(ModelType) == typeof(Session))
            {
                return (ISanatoriumSimpleEntity<ModelType>) new SessionsEntity();
            }

            if (typeof(ModelType) == typeof(Order))
            {
                return (ISanatoriumSimpleEntity<ModelType>) new OrdersEntity();
            }

            if (typeof(ModelType) == typeof(ProgramAppointment))
            {
                return (ISanatoriumSimpleEntity<ModelType>) new ProgramsAppointmentsEntity();
            }

            return new SimpleEntity<ModelType>();
        }

        public static ISanatoriumAggregateEntity<ModelType> getAggregateEntity<ModelType>() where ModelType: BaseModel, new()
        {
            if (typeof(ModelType) == typeof(CustomerAggregate))
            {
                return (ISanatoriumAggregateEntity<ModelType>) new CustomersAggregates();
            }

            return default(ISanatoriumAggregateEntity<ModelType>);
        }

        private static dynamic getEntity<ModelType>() where ModelType: BaseModel, new()
        {
            if (typeof(ModelType) == typeof(Program))
            {
                return (ISanatoriumSimpleEntity<ModelType>) new ProgramsEntity();
            }

            if (typeof(ModelType) == typeof(ProgramStructItem))
            {
                return (ISanatoriumExtendedEntity<ProgramStructItem>) new ProgramsStructs();
            }

            if (typeof(ModelType) == typeof(CustomerAggregate))
            {
                return (ISanatoriumAggregateEntity<CustomerAggregate>) new CustomersAggregates();
            }
                
            return new SimpleEntity<ModelType>();
        }
    }
}
