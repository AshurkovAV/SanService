using System;

namespace SanatoriumEntities.Models.Services
{
    public class CancellationProcedures : BaseModel
    {
        public int?         customer_id            { get; set; }
        public int?         service_id             { get; set; }
        public int?         order_id               { get; set; }
        public int?         customers_schedules_id { get; set; }
        public int?         completed_employee_id  { get; set; }
        public int?         iteration              { get; set; } = 0;
        public DateTime     start_time             { get; set; }
        public DateTime     end_time               { get; set; }
        public DateTime     schedule_date          { get; set; }
        public int?         type_device            { get; set; }
        public int?         type_execution         { get; set; }
        public override string getDatabaseEntityName()
        {
            return "cancellation_procedures";
        }
    }
}
