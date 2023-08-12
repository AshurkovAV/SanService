using System;

namespace SanatoriumEntities.Models.Services
{
    public class GeneralEquipmentItem : BaseModel
    {
        public string       eqp_name               { get; set; }
        public bool?        eqp_mobility           { get; set; } = false;
        public string       eqp_type               { get; set; }
        public int?         place_general_id       { get; set; }
        public string       eqp_data_json          { get; set; }
        public DateTime?    eqp_dasabled_before    { get; set; }
        public override string getDatabaseEntityName()
        {
            return "equipment_general_list";
        }
    }
}
