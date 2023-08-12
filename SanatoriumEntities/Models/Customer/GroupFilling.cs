using System;

namespace SanatoriumEntities.Models.Customer
{
    public class GroupFilling : BaseModel
    {
        public int       customer_id    { get; set; }
        public int       group_id       { get; set; }
        
        public override string getDatabaseEntityName()
        {
            return "groups_filling";
        }
    }
}
