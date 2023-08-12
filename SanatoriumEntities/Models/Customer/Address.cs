namespace SanatoriumEntities.Models.Customer
{
    public class Address : BaseModel
    {
        public int       customer_id         { get; set; }
        public bool      addr_is_legal       { get; set; }
        public int       addr_region_code       { get; set; }
        public string    addr_locality_name   { get; set; }
        public string    addr_locality_type    { get; set; }
        public string    addr_country          { get; set; }
        public string    addr_region           { get; set; }
        public string    addr_district         { get; set; }
        public string    addr_street           { get; set; }
        public string    addr_house_number     { get; set; }
        public string    addr_house_subnumber  { get; set; }
        public string    addr_apartment        { get; set; }
        public string    addr_data_json       { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_addresses";
        }
    }
}
