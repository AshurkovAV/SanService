using System;
using System.Collections.Generic;
using System.Linq;
using SanatoriumEntities.Exceptions;
using SanatoriumEntities.Interfaces.Services;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Helpers;
using SanatoriumEntities.Helpers.Schedule;

namespace SanatoriumEntities.ServicesClasses
{
    public class ServiceSlots : ISlots
    {
        public const int SLOT_DURATION_MINUTES = 5;

        private ServiceSlots() { }
        private static ServiceSlots _instance;
        private static readonly object _lock = new object();
        public static ServiceSlots GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServiceSlots();
                    }
                }
            }
            return _instance;
        }

        public List<ResourceSlot> generateListByParams(SlotsGeneratorParams generatorParams, bool updateDatabase = false)
        {
            List<ResourceSlot> slots = new List<ResourceSlot> { };

            foreach (DateTime day in eachDay(generatorParams.period_start, generatorParams.period_end))
            {
                if (generatorParams.exclude_days.Contains(day))
                {
                    continue;
                }

                foreach (ResourceSlot slot in daySlots(day, generatorParams))
                {
                    slots.Add(slot);
                }
            }

            if (updateDatabase)
            {
                Log.Get().put(Log.DEBUG, String.Format("Обновление БД. [slots.Count]:[{0}]", slots.Count));
                SEntities.ResourcesSlots(true).insertList(slots);
            }

            return slots;
        }

        public List<ResourceSlot> setEmployeeSlotsService(
            List<ResourceSlot> slots,
            Service service
        )
        {
            Log.Get().put(Log.INF, String.Format("<<< Отмечание слотов сотрудника для сервиса. Сервис: [{0}]", service.id));

            List<ResourceSlot> markedSlots = new List<ResourceSlot>();

            foreach (ResourceSlot existedSlot in slots)
            {
                if (existedSlot.resource_category_id == ServiceSessions.EMPLOYEE_CATEGORY)
                {
                    if ((existedSlot.order_id ?? 0) > 0)
                    {
                        continue;
                    }

                    Log.Get().put(Log.TRACE, String.Format("Отмечание слотов для сервиса. Сервис: [{0}]. Слот: [{1}]", service.id, existedSlot.id));

                    int serviceId = service.id ?? 0;

                    existedSlot.service_id = serviceId;
                    SEntities.Slots(true).update(existedSlot);
                    markedSlots.Add(existedSlot);
                }
            }

            Log.Get().put(Log.INF, String.Format(">>> Отмечание слотов сотрудника для сервиса. Сервис: [{0}]", service.id));

            return markedSlots;
        }

        public List<ResourceSlot> unsetEmployeeSlotsService(List<ResourceSlot> slots)
        {
            Log.Get().put(Log.INF, String.Format("<<< Освобождение слотов сотрудника от сервиса"));

            List<ResourceSlot> markedSlots = new List<ResourceSlot>();
            
            foreach (ResourceSlot existedSlot in slots)
            {
                if (existedSlot.resource_category_id == ServiceSessions.EMPLOYEE_CATEGORY)
                {
                    if ((existedSlot.order_id ?? 0) > 0)
                    {
                        continue;
                    }

                    Log.Get().put(Log.TRACE, String.Format("Слот: [{0}]", existedSlot.id));

                    existedSlot.service_id = null;
                    SEntities.Slots(true).update(existedSlot);
                    markedSlots.Add(existedSlot);
                }
            }

            Log.Get().put(Log.INF, String.Format(">>> Освобождение слотов сотрудника от сервиса"));

            return markedSlots;
        }

        public List<ResourceSlot> alignEmployeeSlotsOrdinalsCount(
            List<ResourceSlot> slots,
            int alignment = 1
        )
        {
            Log.Get().put(Log.INF, String.Format("<<< Выравнивание слотов сотрудника. Занчение [{0}]", alignment));

            List<ResourceSlot> markedSlots = new List<ResourceSlot>();

            foreach (ResourceSlot existedSlot in slots)
            {
                if (existedSlot.resource_category_id == ServiceSessions.EMPLOYEE_CATEGORY)
                {
                    if ((existedSlot.order_id ?? 0) > 0)
                    {
                        continue;
                    }

                    if (existedSlot.slot_ordinal > alignment) {
                        SEntities.Slots(true).delete(existedSlot.id ?? 0, true);
                        slots.Remove(existedSlot);
                    }

                    for (int ordinal = 2; ordinal <= alignment; ordinal++)
                    {
                        ResourceSlot newSlot = new ResourceSlot();

                        newSlot.duration_min = existedSlot.duration_min;
                        newSlot.resource_category_id = existedSlot.resource_category_id;
                        newSlot.resource_general_id = existedSlot.resource_general_id;
                        newSlot.slot_start_time = existedSlot.slot_start_time;
                        newSlot.slot_end_time = existedSlot.slot_end_time;
                        newSlot.slot_ordinal = ordinal;

                        try
                        {
                            newSlot.id = SEntities.Slots(true).insert(newSlot);
                            markedSlots.Add(newSlot);
                        }
                        catch (ExecutionException e)
                        {
                            Log.Get().put(Log.TRACE, String.Format("Ошибка. Слот [{0}], Ординал: [{1}]", existedSlot.id, ordinal));
                            Log.Get().put(Log.ERR, e.Message);
                        }
                    }
                }
            }

            Log.Get().put(Log.INF, String.Format(">>> Выравнивание слотов сотрудника. Занчение [{0}]", alignment));

            return markedSlots;
        }

        public Dictionary<int, Dictionary<int, Dictionary<int, List<ResourceSlot>>>> fetchSlotsFromDbByDemandsAsDictionary(
            Order order,
            Service service,
            SlotsFetchingParams fetchingParams,
            Dictionary<int, Dictionary<int, List<int>>> demandsGropus
        )
        {
            Log.Get().put(Log.TRACE, String.Format("<<< Выборка слотов из БД. Период, дней: [{0}]", fetchingParams.period_days));

            if (demandsGropus.Count == 0)
            {
                Log.Get().put(Log.WRN, String.Format(">>> Число групп требований: [0].", demandsGropus.Count));

                return new Dictionary<int, Dictionary<int, Dictionary<int, List<ResourceSlot>>>>();
            }

            Dictionary<int, Dictionary<int, Dictionary<int, List<ResourceSlot>>>> demandsMeted = new Dictionary<int, Dictionary<int, Dictionary<int, List<ResourceSlot>>>>();

            Dictionary<int, ResourceSlot> fetchedSlotsDict = new Dictionary<int, ResourceSlot>();

            List<ResourceSlot> fetchedSlots = prepareParamsAndFetchSlotsFromDb(order, service, fetchingParams, demandsGropus);

            var baseStartTime = fetchingParams.period_start;

            for (int shift = 0; shift < fetchedSlots.Count; shift++)
            {
                bool loop0NeedContinue = false;

                baseStartTime = fetchingParams.period_start.AddMinutes(shift * fetchedSlots.FirstOrDefault().duration_min);

                if (baseStartTime.Day - fetchingParams.period_start.Day > fetchingParams.period_days) break;

                foreach (KeyValuePair<int, Dictionary<int, List<int>>> categoryDemands in demandsGropus)
                {
                    foreach (KeyValuePair<int, List<int>> demandsGroup in categoryDemands.Value)
                    {
                        if (!demandsMeted.ContainsKey(categoryDemands.Key))
                        {
                            demandsMeted.Add(categoryDemands.Key, new Dictionary<int, Dictionary<int, List<ResourceSlot>>>());
                        }

                        if (!demandsMeted[categoryDemands.Key].ContainsKey(demandsGroup.Key))
                        {
                            demandsMeted[categoryDemands.Key].Add(demandsGroup.Key, new Dictionary<int, List<ResourceSlot>>());
                        }

                        foreach (var slotItem in demandsGroup.Value)
                        {
                            if (!demandsMeted[categoryDemands.Key][demandsGroup.Key].ContainsKey(slotItem))
                            {
                                demandsMeted[categoryDemands.Key][demandsGroup.Key].Add(slotItem, new List<ResourceSlot>());
                            }
                        }

                    }
                }

                var query =
                from slots in fetchedSlots
                where (
                        slots.slot_start_time >= baseStartTime
                        && slots.slot_start_time < baseStartTime.AddMinutes(
                            service.svc_duration_slots * fetchedSlots.FirstOrDefault().duration_min
                            )
                    )
                group slots by new { slots.resource_category_id, slots.resource_general_id, slots.slot_ordinal } into slotsGroup
                where slotsGroup.Count() == service.svc_duration_slots
                orderby (slotsGroup.Key.resource_general_id)
                select slotsGroup;

                var mathcedAlts = query.ToList();

                foreach (var item in mathcedAlts)
                {
                    if (demandsMeted.Keys.Contains(item.Key.resource_category_id))
                    {
                        foreach (var demandsAlt in demandsMeted[item.Key.resource_category_id])
                        {
                            foreach (var demand in demandsAlt.Value)
                            {
                                if (demand.Key == item.Key.resource_general_id)
                                {
                                    demandsMeted[item.Key.resource_category_id][demandsAlt.Key][item.Key.resource_general_id] = item.ToList();
                                    break;
                                }
                            }

                        }
                    }
                }

                int customerSchedulePlaceId = 0;

                foreach (var matedCategoryItem in demandsMeted)
                {
                    foreach (var matedItemAlts in matedCategoryItem.Value)
                    {
                        bool altMated = false;

                        try
                        {
                            var item = matedItemAlts.Value.ToList().ElementAt((order.id ?? 0) % matedItemAlts.Value.Count);
                            if (item.Value.Count == service.svc_duration_slots)
                            {
                                if (checkCustomerDbSchedule(order, item.Value.ToList(), ref customerSchedulePlaceId, fetchingParams.max_idle_slots) == 0)
                                {
                                    altMated = true;
                                }
                            }    
                        }
                        catch (System.Exception)
                        {
                            altMated = false;                            
                        }

                        if (!altMated) {
                            foreach (var matedItem in matedItemAlts.Value)
                            {
                                if (matedItem.Value.Count == service.svc_duration_slots)
                                {
                                    if (checkCustomerDbSchedule(order, matedItem.Value.ToList(), ref customerSchedulePlaceId, fetchingParams.max_idle_slots) == 0)
                                    {
                                        altMated = true;
                                    }
                                }
                            }
                        }

                        if (!altMated)
                        {
                            Log.Get().put(Log.DEBUG, String.Format("Сервис [{0}]. Недостаточно свободных слотов ресурсов. Категория [{1}] Группа алтернатив [{2}]", service.id, matedCategoryItem.Key, matedItemAlts.Key));

                            loop0NeedContinue = true;
                            demandsMeted.Clear();

                            break;
                        }

                        if (loop0NeedContinue) break;
                    }

                    if (loop0NeedContinue) break;
                }

                if (loop0NeedContinue) continue;

                return demandsMeted;
            }

            throw new ServiceException(
                String.Format("Сервис [{0}]. Недостаточно свободных слотов ресурсов.", service.id)
            );
        }

        private List<ResourceSlot> prepareParamsAndFetchSlotsFromDb(
            Order order,
            Service service,
            SlotsFetchingParams fetchingDefaultParams,
            Dictionary<int, Dictionary<int, List<int>>> demandsGropus
        )
        {
            Log.Get().put(Log.TRACE, String.Format("<<< Выборка слотов из БД. Период, дней: [{0}]", order.repeat_period_days ?? 1));

            if (demandsGropus.Count == 0)
            {
                Log.Get().put(Log.WRN, String.Format(">>> Число групп требований: [0].", demandsGropus.Count));

                return new List<ResourceSlot>();
            }

            SlotsFetchingParams fetchingParams = new SlotsFetchingParams();
            fetchingParams.period_days = order.repeat_period_days ?? 1;
            fetchingParams.days_of_week = fetchingDefaultParams.days_of_week;
            fetchingParams.period_start = fetchingDefaultParams.period_start;
            fetchingParams.service_id = fetchingDefaultParams.service_id;
            fetchingParams.preferred_resources_ids = fetchingDefaultParams.preferred_resources_ids;
            fetchingParams.orderAllSlotsOrdinals = fetchingDefaultParams.orderAllSlotsOrdinals;

            fetchingParams.preferred_time_start = new DateTime(
                fetchingDefaultParams.period_start.Year,
                fetchingDefaultParams.period_start.Month,
                fetchingDefaultParams.period_start.Day,
                fetchingDefaultParams.preferred_time_start.Hour,
                fetchingDefaultParams.preferred_time_start.Minute,
                fetchingDefaultParams.preferred_time_start.Second
            );

            fetchingParams.preferred_time_end = new DateTime(
                fetchingDefaultParams.period_start.Year,
                fetchingDefaultParams.period_start.Month,
                fetchingDefaultParams.period_start.Day,
                fetchingDefaultParams.preferred_time_end.Hour,
                fetchingDefaultParams.preferred_time_end.Minute,
                fetchingDefaultParams.preferred_time_end.Second
            );

            fetchingParams.service_id = fetchingDefaultParams.service_id;
            if ((service.svc_is_group ?? false) && ((fetchingDefaultParams.service_id ?? 0) == 0))
            {
                fetchingParams.service_id = service.id;
            }

            List<ResourceSlot> fetchedSlots = new List<ResourceSlot>();

            foreach (KeyValuePair<int, Dictionary<int, List<int>>> categoryDemands in demandsGropus)
            {
                foreach (KeyValuePair<int, List<int>> demandsGroup in categoryDemands.Value)
                {
                    fetchingParams.resource_category_id = categoryDemands.Key;
                    fetchingParams.resource_general_ids = demandsGroup.Value;

                    List<ResourceSlot> slotsFromDb = ServiceSlots.GetInstance().fetchSlotsFromDbByFetchingParams(fetchingParams);
                    fetchedSlots.AddRange(slotsFromDb);
                }
            }

            Log.Get().put(Log.TRACE, String.Format(">>> Возврат слотов из БД. Всего: [{0}]", fetchedSlots.Count));

            return fetchedSlots;
        }

        private List<ResourceSlot> fetchSlotsFromDbByFetchingParams(SlotsFetchingParams fetchingParams)
        {
            try
            {
                List<ResourceSlot> result = ObjectSpawnerHelper<ResourceSlot>.spawnModelObjectBySqlStatementsListBySqlStatement(
                    () => new ResourceSlot(),
                    generateSQLStatementByFetchingParams(fetchingParams)
                );

                Log.Get().put(Log.DEBUG, String.Format(">>> выбрано [{0}] записей.", result.Count));

                return result;

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        protected IEnumerable<DateTime> eachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        protected IEnumerable<ResourceSlot> daySlots(DateTime day, SlotsGeneratorParams generatorParams)
        {
            for (int slotDayOrdinal = 0; slotDayOrdinal < generatorParams.day_duration_slots; slotDayOrdinal++)
            {
                for (int slotOrdinal = 1; slotOrdinal <= generatorParams.resource_capacity; slotOrdinal++)
                {
                    ResourceSlot slot = new ResourceSlot();

                    DateTime start = new DateTime(
                        day.Year,
                        day.Month,
                        day.Day,
                        generatorParams.day_start_at_hour,
                        generatorParams.day_start_at_min,
                        0
                    ); // год, месяц, день, час, минута, секунда

                    slot.slot_start_time = start.AddMinutes(slotDayOrdinal * generatorParams.slot_duration_min);
                    slot.slot_end_time = slot.slot_start_time.AddSeconds(generatorParams.slot_duration_min * 60);

                    slot.resource_category_id = generatorParams.resource_category_id;
                    slot.resource_general_id = generatorParams.resource_general_id;
                    slot.slot_ordinal = slotOrdinal;

                    yield return slot;
                }

            }
        }

        private string generateSQLStatementByFetchingParams(SlotsFetchingParams fetchingParams)
        {
            List<int> resourcesIds = new List<int>();

            if (fetchingParams.preferred_resources_ids.ContainsKey(fetchingParams.resource_category_id ?? 0))
            {
                resourcesIds = fetchingParams.preferred_resources_ids[fetchingParams.resource_category_id ?? 0];
            }
            else
            {
                resourcesIds = fetchingParams.resource_general_ids;
            }

            var serviceCondition = "";
            if ((fetchingParams.service_id ?? 0) > 0)
            {
                serviceCondition = $" AND (service_id={fetchingParams.service_id} OR ISNULL(service_id,0)=0) ";
            }

            var ordinalsCondition = "";
            if (fetchingParams.resource_category_id == ServiceSessions.EMPLOYEE_CATEGORY && fetchingParams.orderAllSlotsOrdinals)
            {
                ordinalsCondition = $" AND slot_ordinal=1 ";
            }

            string statement =
                $"SELECT * FROM resources_slots WHERE active=1 AND ISNULL(order_id,0)=0 AND resource_category_id={fetchingParams.resource_category_id ?? 0}"
                    + serviceCondition
                    + ordinalsCondition
                    + $" AND resource_general_id IN ({String.Join(",", resourcesIds)})"
                    + $" AND slot_start_time>='{fetchingParams.period_start.ToString("yyyyMMdd HH:mm:ss")}' AND DATEDIFF(DAY, '{fetchingParams.period_start.ToString("yyyyMMdd HH:mm:ss")}', slot_end_time)<{fetchingParams.period_days}"
                    + $" AND (DATEPART(HOUR, '{fetchingParams.preferred_time_start.ToString("yyyyMMdd HH:mm:ss")}')*60 + DATEPART(MINUTE, '{fetchingParams.preferred_time_start.ToString("yyyyMMdd HH:mm:ss")}')) <= (DATEPART(HOUR, slot_start_time)*60 + DATEPART(MINUTE, slot_start_time))"
                    + $" AND (DATEPART(HOUR, '{fetchingParams.preferred_time_end.ToString("yyyyMMdd HH:mm:ss")}')*60 + DATEPART(MINUTE, '{fetchingParams.preferred_time_end.ToString("yyyyMMdd HH:mm:ss")}')) > (DATEPART(HOUR, slot_start_time)*60 + DATEPART(MINUTE, slot_start_time))"
                    + $" AND DATEPART(dw, slot_start_time) IN ({String.Join(",", fetchingParams.days_of_week)})"
                + " ORDER BY resource_category_id,resource_general_id,slot_ordinal,slot_start_time"
            ;

            Log.Get().put(Log.DEBUG, String.Format("Запрос выборки слотов из БД на основе параметров: [{0}]", statement));

            return statement;
        }

        private int checkCustomerDbSchedule(Order order, List<ResourceSlot> slots, ref int placeId, int maxIdleSlots)
        {
            Log.Get().put(Log.DEBUG, String.Format("<<< Ордер [{0}]. Проверка в расписании клиента", order.id));

            if (slots.Count == 0)
            {
                Log.Get().put(Log.WRN, String.Format("Ордер [{0}]. Не переданы слоты на проверку в расписании клиента. return [true]", order.id));

                return 0;
            }

            string scheduleDate = slots.FirstOrDefault().slot_start_time.ToString("yyyyMMdd");

            var dayCustomerSchedule = SEntities.CustomersSchedules().selectList(
                $"customer_id={order.customer_id} AND schedule_date='{scheduleDate}' AND is_skipped=0 AND ISNULL(moved_to,0)=0",
                "id"
            );

            if (dayCustomerSchedule.Count == 0)
            {
                Log.Get().put(Log.DEBUG, String.Format(">>> Ордер [{0}]. Расписание свободно в этот день.", order.id));
                return 0;
            }

            var slotsOrdered =
                from slot in slots
                orderby slot.slot_start_time
                select slot;

            var dayCustomerScheduleOrdered =
                from item in dayCustomerSchedule
                orderby item.start_time
                select item;

            if (slots.FirstOrDefault().resource_category_id == ServiceSessions.PLACE_CATEGORY) {
                placeId = slots.FirstOrDefault().resource_general_id;
            }

            Int32 startSlotStamp = (Int32)(slotsOrdered.FirstOrDefault().slot_start_time.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            Int32 endSlotStamp = (Int32)(slotsOrdered.LastOrDefault().slot_end_time.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;

            Int32 scheduleStartStamp = (Int32)(dayCustomerScheduleOrdered.FirstOrDefault().start_time.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            Int32 scheduleEndStamp = (Int32)(dayCustomerScheduleOrdered.LastOrDefault().end_time.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;

            if ((startSlotStamp - scheduleEndStamp) >= (SLOT_DURATION_MINUTES * maxIdleSlots))
            {
                Log.Get().put(Log.TRACE, String.Format("Ордер [{0}]. Разница слотов и расписания >60 минут. return [false]", order.id));

                return -1;
            }

            if ((scheduleStartStamp - endSlotStamp) >= (SLOT_DURATION_MINUTES * maxIdleSlots))
            {
                Log.Get().put(Log.TRACE, String.Format("Ордер [{0}]. Разница слотов и расписания >60 минут. return [false]", order.id));

                return 1;
            }

            foreach (var scheduleItem in dayCustomerScheduleOrdered)
            {
                DateTime endTime = scheduleItem.end_time;

                if (placeId > 0) {
                    if (scheduleItem.place_general_id != placeId) {
                        endTime = scheduleItem.end_time.AddMinutes(3 * SLOT_DURATION_MINUTES);
                    }
                }

                foreach (ResourceSlot slot in slots)
                {
                    if (slot.slot_start_time >= scheduleItem.start_time
                        && slot.slot_end_time <= endTime)
                    {
                        Log.Get().put(Log.TRACE, String.Format(
                            "Ордер [{0}]. Слот id:[{1}] не подходит по времени (с {2} по {3}). return [false]",
                            order.id,
                            slot.id,
                            slot.slot_start_time.ToString("yyyyMMdd HH:mm:ss"),
                            slot.slot_end_time
                        ));

                        return -1;
                    }
                }
            }

            Log.Get().put(Log.DEBUG, String.Format(">>> Ордер [{0}]. Слоты проверены.", order.id));

            return 0;
        }
    }
}
