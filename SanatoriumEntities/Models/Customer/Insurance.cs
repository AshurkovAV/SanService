using System;
namespace SanatoriumEntities.Models.Customer
{
    public class Insurance : BaseModel
    {
        public int       customer_id            { get; set; }
        public int?      insdoc_type_id         { get; set; }
        public string    insdoc_enp             { get; set; }
        public string    insdoc_serial_number   { get; set; }
        public string    insdoc_number          { get; set; }
        public string    insdoc_org_name        { get; set; }
        public DateTime? insdoc_date            { get; set; }
        public DateTime? insdoc_date_end        { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_insurances";
        }
    }
}
