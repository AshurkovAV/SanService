using System;

namespace SanatoriumEntities.Models.Customer
{
    public class Gropus : BaseModel
    {
        public string    grp_title            { get; set; }
        public int       grp_max_size         { get; set; } = 0;
        public int       grp_members_count    { get; set; } = 0;
        public DateTime? grp_date             { get; set; }
        public string    grp_description      { get; set; }
        
        public override string getDatabaseEntityName()
        {
            return "customers_groups";
        }
    }
}
