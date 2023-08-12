using System;

namespace SanatoriumEntities.Models
{
    public abstract class BaseModel
    {
        public int? id { get; set; }
        public bool active { get; set; } = true;
        public int? ip4 { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string updated_by { get; set; }
        public abstract string getDatabaseEntityName();
    }
}
