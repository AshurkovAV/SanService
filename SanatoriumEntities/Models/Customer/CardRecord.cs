using System;

namespace SanatoriumEntities.Models.Customer
{
    public class CardRecord : BaseModel
    {
        public int       patient_card_id        { get; set; }
        public int?      employee_general_id    { get; set; }
        public int?      csession_id        { get; set; }
        public string    rec_type           { get; set; }
        public string    rec_title          { get; set; }
        public string    rec_result         { get; set; }
        public DateTime? rec_date           { get; set; }
        public DateTime? rec_date_next      { get; set; }
        public DateTime? rec_date_end       { get; set; }
        public string    rec_data_json      { get; set; }
        public override string getDatabaseEntityName()
        {
            return "patients_cards_records";
        }
    }
}
