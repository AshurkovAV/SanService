using System;
using System.Collections.Generic;

namespace SanatoriumEntities.Helpers.Schedule
{
    public class SlotsFetchingParams
    {
        public int?             resource_category_id  { get; set; }
        public List<int>        resource_general_ids  { get; set; }
        public List<int>        days_of_week          { get; set; } = new List<int>(){ 1, 2, 3, 4, 5, 6, 7 };
        public DateTime         preferred_time_start  { get; set; }
        public DateTime         preferred_time_end    { get; set; }
        public DateTime         period_start          { get; set; }
        public int              period_days           { get; set; } = 14;
        public int              max_idle_slots        { get; set; } = 6;
        public int              slots_count           { get; set; } = 3;
        public int?             service_id            { get; set; }
        public bool             orderAllSlotsOrdinals { get; set; } = false;
        public Dictionary<int, List<int>> preferred_resources_ids  { get; set; } = new Dictionary<int, List<int>>();
    }
}