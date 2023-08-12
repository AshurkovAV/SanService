using System;
namespace SanatoriumEntities.Models.Session
{
    public class Session : BaseModel
    {
        public int       customer_id            { get; set; }
        public DateTime? sess_date_started      { get; set; }
        public DateTime? sess_date_ended        { get; set; }
        public int?      sess_duration_days     { get; set; }
        public string    sess_data_json         { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_sessions";
        }
    }
}
