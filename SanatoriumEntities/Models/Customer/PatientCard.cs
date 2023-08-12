using System;

namespace SanatoriumEntities.Models.Customer
{
    public class PatientCard : BaseModel
    {
        public int       customer_id            { get; set; }
        public int?      employee_general_id    { get; set; }
        public int?      pcard_type_id      { get; set; }
        public DateTime? pcard_date         { get; set; }
        public string    pcard_data_json    { get; set; }
        public override string getDatabaseEntityName()
        {
            return "patients_cards";
        }
    }
}
