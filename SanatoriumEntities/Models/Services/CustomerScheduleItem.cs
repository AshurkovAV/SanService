using System;

namespace SanatoriumEntities.Models.Services
{
    public class CustomerScheduleItem : BaseModel
    {
        public int?         customer_id            { get; set; } = 0;
        public int?         csession_id            { get; set; }
        public int?         service_id             { get; set; } = 0;
        public int?         service_category_id    { get; set; } = 0;
        public int?         order_id               { get; set; } = 0;
        public int?         iteration              { get; set; } = 0;
        public int?         employee_general_id    { get; set; } = 0;
        public int?         completed_employee_id { get; set; }
        public int?         place_general_id       { get; set; } = 0;
        public bool         is_published           { get; set; } = false;
        public bool         is_skipped             { get; set; } = false;
        public bool         is_executed            { get; set; } = false;
        public bool         is_moved               { get; set; } = false;
        public int?         moved_to               { get; set; }
        public DateTime     start_time             { get; set; }
        public DateTime     end_time               { get; set; }
        public DateTime     schedule_date          { get; set; }
        public string       schedule_description   { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_schedules";
        }
    }
}
