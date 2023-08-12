using System;
namespace SanatoriumEntities.Models.Session
{
    public class Contract : BaseModel
    {
        public int       csession_id      { get; set; }
        public DateTime? ct_date          { get; set; }
        public string    ct_number        { get; set; }
        public string    ct_name          { get; set; }
        public string    ct_data_json     { get; set; }
        public override string getDatabaseEntityName()
        {
            return "contracts";
        }
    }
}
