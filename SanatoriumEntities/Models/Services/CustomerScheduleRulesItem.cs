namespace SanatoriumEntities.Models.Services
{
    public class CustomerScheduleRulesItem : BaseModel
    {
        public int?         svc_category_id         { get; set; } = 0;
        public int?         next_svc_category_id    { get; set; } = 0;
        public int?         service_id              { get; set; } = 0;
        public int?         next_service_id         { get; set; } = 0;
        public bool         is_together             { get; set; } = false;
        public bool         is_require              { get; set; } = false;
        public bool         is_deny                 { get; set; } = false;
        public bool         is_daily                { get; set; } = false;
        public int?         linked_rule_id          { get; set; } = 0;

        public override string getDatabaseEntityName()
        {
            return "services_schedule_rules";
        }
    }
}
