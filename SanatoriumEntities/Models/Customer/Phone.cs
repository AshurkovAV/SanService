namespace SanatoriumEntities.Models.Customer
{
    public class Phone : BaseModel
    {
        public int       customer_id         { get; set; }
        public string    phone_number       { get; set; }
        public string    phone_type   { get; set; }
        public bool      main       { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_phones";
        }
    }
}
