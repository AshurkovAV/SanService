using System;
namespace SanatoriumEntities.Models.Customer
{
    public class Document : BaseModel
    {
        public int       customer_id         { get; set; }
        public int       doc_type_code       { get; set; }
        public string    doc_type_name       { get; set; }
        public string    doc_serial_number   { get; set; }
        public string    doc_number          { get; set; }
        public string    doc_unit_name       { get; set; }
        public string    doc_unit_code       { get; set; }
        public DateTime  doc_date            { get; set; }
        public DateTime? doc_date_end        { get; set; }
        public int       doc_region_code     { get; set; }
        public string    doc_data_json       { get; set; }
        public override string getDatabaseEntityName()
        {
            return "customers_documents";
        }
    }
}
