using System;
using System.Collections.Generic;
using SanatoriumEntities.ServicesClasses;

namespace SanatoriumEntities.Helpers.Schedule
{
    public class SlotsGeneratorParams
    {
        public int              resource_category_id  { get; set; } /*100:'employee',200:'place'*/
        public int              resource_general_id   { get; set; }
        public int              resource_capacity     { get; set; } = 1;
        public int              day_start_at_hour     { get; set; } = 8;
        public int              day_start_at_min      { get; set; } = 0;
        public int              slot_duration_min     { get; set; } = ServiceSlots.SLOT_DURATION_MINUTES;
        public int              day_duration_slots    { get; set; } = 108;
        public DateTime         period_start          { get; set; }
        public DateTime         period_end            { get; set; }
        public List<DateTime>   exclude_days { get; set; }
    }
}