using System;
namespace SanatoriumEntities.Models.Session
{
    public class ProgramAppointment : BaseModel
    {
        public int       csession_id            { get; set; }
        public int?      contract_id            { get; set; }
        public int       program_id             { get; set; }
        public int       employee_general_id    { get; set; }
        public DateTime? ap_start_date    { get; set; }
        public DateTime? ap_end_date      { get; set; }
        public bool?     ap_is_started    { get; set; }
        public bool?     ap_is_finished   { get; set; }
        public bool      has_orders       { get; set; } = false;
        public override string getDatabaseEntityName()
        {
            return "programs_appointments";
        }
    }
}
