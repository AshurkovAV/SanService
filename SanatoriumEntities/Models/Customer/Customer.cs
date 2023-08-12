using System;

namespace SanatoriumEntities.Models.Customer
{
    public class Customer : BaseModel
    {
        public string cust_internal_code    { get; set; }
        public string cust_name             { get; set; }
        public string cust_middle_name      { get; set; }
        public string cust_surname          { get; set; }
        public string cust_snils            { get; set; }
        public string cust_enp              { get; set; }
        public int? cust_region_code        { get; set; }
        public int? cust_special_notes_id   { get; set; }
        public DateTime cust_birthday       { get; set; }
        public bool? cust_is_male           { get; set; }
        public bool? cust_is_adult          { get; set; }
        public bool? cust_has_disabilities  { get; set; }
        public string cust_phone            { get; set; }
        public string cust_data_json        { get; set; }
        public string cust_description      { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers";
        }
    }
}
