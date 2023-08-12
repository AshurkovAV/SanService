using System;
namespace SanatoriumEntities.Models.Program
{
    public class Program : BaseModel
    {       
        public string       p_name                      { get; set; }
        public string       p_code                      { get; set; }
        public int?         p_help_kind                 { get; set; }
        public int?         p_help_form                 { get; set; }
        public int?         p_help_conditions           { get; set; }
        public string       p_help_phase                { get; set; }
        public string       p_stage                     { get; set; }
        public string       p_description               { get; set; }
        public int?         p_duration_days             { get; set; }
        public bool?        p_adult                     { get; set; }
        public bool?        p_child                     { get; set; }
        public bool?        p_is_for_male               { get; set; }
        public bool?        p_is_for_female             { get; set; }
        public bool?        p_is_budget_finance         { get; set; }
        public bool?        p_is_custom                 { get; set; }
        public int?         p_expected_orders_count     { get; set; }
        public int?         p_cost_rub_minor            { get; set; }
        public override string getDatabaseEntityName()
        {
            return "programs_list";
        }
    }
}
