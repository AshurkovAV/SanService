namespace SanatoriumEntities.Models.Catalogs
{
    public class OrderCategory : BaseModel
    {
        public string    order_cat_name { get; set; }
        public string    name_view      { get; set; }
        public override string getDatabaseEntityName()
        {
            return "catalog_orders_categories";
        }
    }
}
