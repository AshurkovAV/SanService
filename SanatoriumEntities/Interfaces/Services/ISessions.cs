using System.Collections.Generic;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Helpers.Schedule;

namespace SanatoriumEntities.Interfaces.Services
{
    public interface ISessions
    {
        Order dispathcOrder(Order order, SlotsFetchingParams fetchingDefaultParams, bool updateDatabase = false);
        CustomerScheduleItem dispathcOrderIteration(
            Order order,
            int iteration,
            SlotsFetchingParams fetchingParams,
            bool updateDatabase = false
        );
        Order releaseOrderSchedule(Order order);
        Order releaseOrderIterationSchedule(Order order, int iteration);
        List<ResourceSlot> releaseSlotsList(List<ResourceSlot> slots, bool updateDatabase = false);
        Order registerExecutedOrderIteration(Order order, int? completedEmployeeId = null, int iteration = 0);
    }
}
