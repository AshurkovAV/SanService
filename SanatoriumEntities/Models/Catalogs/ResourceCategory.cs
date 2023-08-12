namespace SanatoriumEntities.Models.Catalogs
{
    public class ResourceCategory : BaseModel
    {
        public string    r_type_char    { get; set; }
        public string    r_name         { get; set; }
        public string    name_view      { get; set; }
        public override string getDatabaseEntityName()
        {
            return "catalog_resources_categories";
        }
    }
}
