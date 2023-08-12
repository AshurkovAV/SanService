using System;
using System.Linq;
using System.Collections.Generic;
namespace SanatoriumEntities.Models.Services
{
    public class Order : BaseModel
    {
        public int          service_id                    { get; set; }
        public int          employee_general_id           { get; set; }
        public int?         customer_id                   { get; set; }
        public int?         csession_id                   { get; set; }
        public int?         contract_id                   { get; set; }
        public int?         program_id                    { get; set; }
        public int?         program_appointment_id        { get; set; }
        public int?         order_categoty_id             { get; set; }
        public int?         repeats_count                 { get; set; }
        public int?         repeated_count                { get; set; } = 0;
        public int?         repeat_period_days            { get; set; } = 1;
        public bool?        is_planned                    { get; set; }
        public bool?        is_partially_planned          { get; set; }
        public bool?        is_closed                     { get; set; }
        public bool?        is_approved                   { get; set; }
        public int?         cost_rub_minor                { get; set; }
        public string       order_description             { get; set; }
        public DateTime?    executed_at                   { get; set; }
        private HashSet<int>   undispatchedIterations = new HashSet<int>();
        private HashSet<int>   dispatchedIterations = new HashSet<int>();

        public override string getDatabaseEntityName()
        {
            return "orders";
        }

        public HashSet<int> getUndispatchedIterations()
        {
            if (undispatchedIterations.Count == 0 && dispatchedIterations.Count == 0) {
                for (int iteration = 1; iteration <= repeats_count; iteration++) {
                    undispatchedIterations.Add(iteration);
                }
            }
            
            return undispatchedIterations;
        }

        public HashSet<int> getDispatchedIterations()
        {
            return dispatchedIterations;
        }

        public void markIterationDispathed(int iteration) {
            if (getUndispatchedIterations().Contains(iteration)) {
                undispatchedIterations.Remove(iteration);
                dispatchedIterations.Add(iteration);
            }
        }
    }
}
