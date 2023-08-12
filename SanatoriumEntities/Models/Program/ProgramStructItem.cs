using System;
namespace SanatoriumEntities.Models.Program
{
    public class ProgramStructItem : BaseModel
    {
        public int      program_id          { get; set; }
        public int      service_id          { get; set; }
        public int?     alt_svc_prg_id      { get; set; }
        public int?     alt_group_id        { get; set; }
        public int      repeats_count       { get; set; }
        public int?     repeats_freq        { get; set; }
        public int?     sps_cost_rub_minor  { get; set; }
        public override string getDatabaseEntityName()
        {
            return "programs_services_struct";
        }
    }
}
