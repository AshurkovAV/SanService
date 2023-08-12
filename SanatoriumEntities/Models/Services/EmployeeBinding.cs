using System;

namespace SanatoriumEntities.Models.Services
{
    public class EmployeeBinding : AbstractResourceBinding
    {
        public int?       employee_general_id     { get; set; }
        public int?       employee_id             { get; set; }
        public override string getDatabaseEntityName()
        {
            return "employees_bindings";
        }
    }
}
