using System;

namespace SanatoriumEntities.Models.Services
{
    public class AbstractResourceBinding : BaseModel
    {
        public DateTime?      binding_start     { get; set; }
        public DateTime?      binding_end       { get; set; }
        public override string getDatabaseEntityName()
        {
            return "";
        }
    }
}
