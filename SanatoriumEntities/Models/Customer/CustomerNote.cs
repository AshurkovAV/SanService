namespace SanatoriumEntities.Models.Customer
{
    public class CustomerNote : BaseModel
    {
        public string    note       { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_special_notes";
        }
    }
}
