using System;

namespace SanatoriumEntities.Models.Services
{
    public class EmployeeResource : BaseModel
    {
        public string       empl_internal_code      { get; set; }
        public string       empl_name               { get; set; }
        public string       empl_middle_name        { get; set; }
        public string       empl_surname            { get; set; }
        public DateTime?    empl_birthday           { get; set; }
        public bool?        empl_is_male            { get; set; } = false;
        public string       empl_rank               { get; set; }
        public string       empl_snils              { get; set; }
        public string       empl_phone              { get; set; }
        public string       empl_data_json          { get; set; } = "{}";
        public DateTime?    empl_dasabled_before    { get; set; }
        public override string getDatabaseEntityName()
        {
            return "employees";
        }
    }
}
