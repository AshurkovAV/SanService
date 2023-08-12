using System.Collections.Generic;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Helpers.Schedule;

namespace SanatoriumEntities.Interfaces.Services
{
    public interface ISlots
    {
        List<ResourceSlot> generateListByParams(SlotsGeneratorParams generatorParams, bool updateDatabase = false);

        Dictionary<int, Dictionary<int, Dictionary<int, List<ResourceSlot>>>> fetchSlotsFromDbByDemandsAsDictionary(
            Order order,
            Service service,
            SlotsFetchingParams fetchingDefaultParams,
            Dictionary<int, Dictionary<int, List<int>>> demandsGropus
        );

        List<ResourceSlot> setEmployeeSlotsService(
            List<ResourceSlot> slots,
            Service service
        );

        List<ResourceSlot> alignEmployeeSlotsOrdinalsCount(
            List<ResourceSlot> slots,
            int alignment = 1
        );

        List<ResourceSlot> unsetEmployeeSlotsService(List<ResourceSlot> slots);
    }
}
