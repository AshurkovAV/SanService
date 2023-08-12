using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanatoriumEntities.Models.View
{
    public class EmployeesProceduresList : BaseModel
    {
        public bool      is_executed      { get; set; }
        public int?      csession_id      { get; set; }
        public int?      customer_id      { get; set; }
        public string    cust_surname     { get; set; }
        public string    cust_name        { get; set; }
        public string    cust_middle_name { get; set; }
        public DateTime  dt               { get; set; }
        public DateTime  start_time       { get; set; }
        public DateTime  end_time         { get; set; }
        public string    start_time_text  { get; set; }
        public string    svc_name         { get; set; }
        public string    cab              { get; set; }
        public int?      order_id         { get; set; }
        public string    iteration        { get; set; }
        public string    contract         { get; set; }
        public bool?     p_is_budget_finance { get; set; }
        public int?      employee_general_id { get; set; }
       

        public override string getDatabaseEntityName()
        {
            return "view_employees_procedures_list";
        }
    }
}
