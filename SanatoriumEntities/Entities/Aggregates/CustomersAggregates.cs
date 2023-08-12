using System;
using System.Collections.Generic;
using SanatoriumEntities.Models.Customer;
using SanatoriumEntities.Helpers;
using SanatoriumEntities.Interfaces;
using CustomerAggregate = SanatoriumEntities.Models.Customer.CustomerAggregate;

namespace SanatoriumEntities.Entities.Aggregates
{
    public class CustomersAggregates : AbsrtactSanatoriumAggregate<CustomerAggregate, CustomerAggregate>
    {
        public override List<CustomerAggregate> selectList(string filter, List<string> fields, List<string> orderByFields)
        {
            return selectList(filter,String.Join(",", orderByFields));
        }

        public override CustomerAggregate select(int id)
        {
            var customerAggregate = this.getModel();

            var customerEntity  = new SimpleEntity<Customer>();
            var phoneEntity     = new SimpleEntity<Phone>();
            var addressEntity   = new SimpleEntity<Address>();
            var documentEntity  = new SimpleEntity<Document>();
            var insuranceEntity = new SimpleEntity<Insurance>();

            var customer = customerEntity.select(id);

            int? customerId = customer.id ?? 0;

            if (customer.id > 0) {
                customerAggregate = ObjectCopier<Customer, CustomerAggregate>.CopyProperies(customer, customerAggregate);

                string filter = $"customer_id={customer.id} AND active=1";

                customerAggregate.phones    =  phoneEntity.selectList(filter);
                customerAggregate.addresses =  addressEntity.selectList(filter);
                customerAggregate.documents =  documentEntity.selectList(filter);
                customerAggregate.insurances = insuranceEntity.selectList(filter);

                return customerAggregate;

            } else {
                return default(CustomerAggregate);
            }
        }

        public override List<CustomerAggregate> selectList(string filter, string orderByFields = "id")
        {
            var result = new List<CustomerAggregate>();

            var customerEntity  = new SimpleEntity<Customer>();
            var phoneEntity     = new SimpleEntity<Phone>();
            var addressEntity   = new SimpleEntity<Address>();
            var documentEntity  = new SimpleEntity<Document>();
            var insuranceEntity = new SimpleEntity<Insurance>();

            var customers = customerEntity.selectList(filter, orderByFields);

            if (null == customers) {
                return default(List<CustomerAggregate>);
            }

            foreach (var customer in customers)
            {
                var customerAggregate = this.getModel();

                customerAggregate = ObjectCopier<Customer, CustomerAggregate>.CopyProperies(customer, customerAggregate);

                filter = $"customer_id={customer.id} AND active=1";

                customerAggregate.phones     =  phoneEntity.selectList(filter, "id");
                customerAggregate.addresses  =  addressEntity.selectList(filter, "id");
                customerAggregate.documents  =  documentEntity.selectList(filter, "id");
                customerAggregate.insurances = insuranceEntity.selectList(filter, "id");

                result.Add(customerAggregate);
            }

            return result;
        }
    }
}
