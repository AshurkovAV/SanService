using System.Collections.Generic;

namespace SanatoriumEntities.Models.Customer
{
    public class CustomerAggregate : Customer
    {
        public List<Document> documents { get; set; }
        public List<Address> addresses { get; set; }
        public List<Phone> phones { get; set; }
        public List<Insurance> insurances { get; set; }
    }
}
