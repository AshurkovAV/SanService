using System;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Models;
using SanatoriumEntities.Models.Session;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Models.Program;
using SanatoriumEntities.Helpers.Schedule;
using SanatoriumEntities.Interfaces;
using SanatoriumEntities.Interfaces.Services;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Exceptions;
using SanatoriumEntities.Entities.Overriden;

namespace SanatoriumEntities.ServicesClasses
{
    public class ServiceSessions : ISessions
    {
        public const int EMPLOYEE_CATEGORY = 100;
        public const int PLACE_CATEGORY = 200;
        public const int EQUIPMENT_CATEGORY = 300;

        private ServiceSessions() { }
        private static ServiceSessions _instance;
        private static readonly object _lock = new object();

        public static ServiceSessions GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServiceSessions();
                    }
                }
            }
            return _instance;
        }

        public Order dispathcOrder(
            Order order,
            SlotsFetchingParams fetchingDefaultParams,
            bool updateDatabase = false
        ) {
            Log.Get().put(Log.WRN, String.Format("<<< Ордер [{0}]. Планирование ресурсов.",  order.id));

            int iterationsPlanned = 0;
            
            DateTime stratedAt = fetchingDefaultParams.period_start;

            for (int iteration = 1; iteration <= order.repeats_count; iteration++)
            {
                Log.Get().put(Log.DEBUG, String.Format("Ордер [{0}]. <<< Итерация [{1}]", order.id, iteration));

                DateTime matchingStartTime = (new DateTime(
                    stratedAt.Year,
                    stratedAt.Month,
                    stratedAt.Day,
                    fetchingDefaultParams.preferred_time_start.Hour,
                    fetchingDefaultParams.preferred_time_start.Minute,
                    fetchingDefaultParams.preferred_time_start.Second
                )).AddDays((iteration - 1) * (order.repeat_period_days ?? 1));
                fetchingDefaultParams.period_start = matchingStartTime;

                try {
                    var customerScheduleItem = dispathcOrderIteration(order, iteration, fetchingDefaultParams, updateDatabase);

                    if (customerScheduleItem != null) {
                        if (order.id == customerScheduleItem.order_id && customerScheduleItem.iteration == iteration) {
                            iterationsPlanned += 1;
                            order.is_partially_planned = true;
                            order.markIterationDispathed(iteration);
                        }
                    }
                } catch (ServiceException e) {
                    Log.Get().put(Log.WRN, e.Message);
                }
            }

            int maxDispatchedIteration = 0;

            if (order.getDispatchedIterations().Count > 0) {
                maxDispatchedIteration = order.getDispatchedIterations().Max();
            }

            for (int extraAttempt = 1; (maxDispatchedIteration + extraAttempt) <= fetchingDefaultParams.period_days; extraAttempt++) {
                if (order.getUndispatchedIterations().Count == 0) {
                    break;
                }

                int iteration = order.getUndispatchedIterations().Min();

                Log.Get().put(Log.DEBUG, String.Format("Ордер [{0}]. <<< Доп попытка. Итерация [{1}]", order.id, iteration));

                DateTime matchingStartTime = (new DateTime(
                    stratedAt.Year,
                    stratedAt.Month,
                    stratedAt.Day,
                    fetchingDefaultParams.preferred_time_start.Hour,
                    fetchingDefaultParams.preferred_time_start.Minute,
                    fetchingDefaultParams.preferred_time_start.Second
                )).AddDays((maxDispatchedIteration + extraAttempt - 1) * (order.repeat_period_days ?? 1));
                fetchingDefaultParams.period_start = matchingStartTime;

                try {
                    var customerScheduleItem = dispathcOrderIteration(order, iteration, fetchingDefaultParams, updateDatabase);

                    if (customerScheduleItem != null) {
                        if (order.id == customerScheduleItem.order_id && customerScheduleItem.iteration == iteration) {
                            iterationsPlanned += 1;
                            order.is_partially_planned = true;
                            order.markIterationDispathed(iteration);
                            maxDispatchedIteration = order.getDispatchedIterations().Max();
                        }
                    }
                } catch (ServiceException e) {
                    Log.Get().put(Log.WRN, e.Message);
                }
            }

            if (order.repeats_count == iterationsPlanned) {
                order.is_planned = true;
                order.is_partially_planned = false;
                Log.Get().put(Log.INF, String.Format("Ордер [{0}]. Статус: запланировн [is_planned = true, is_partially_planned = false]", order.id));
            }

            if (updateDatabase) {
                Log.Get().put(Log.DEBUG, String.Format("Ордер [{0}]. Обновление БД.", order.id));
                SEntities.Orders(true).update(order);
            }

            Log.Get().put(Log.WRN, String.Format(">>> Ордер [{0}]. Планирование ресурсов завершено.",  order.id));

            return order;
        }

        public CustomerScheduleItem dispathcOrderIteration(
            Order order,
            int iteration,
            SlotsFetchingParams fetchingParams,
            bool updateDatabase = false
        ) {
            Log.Get().put(Log.DEBUG, String.Format("<<< Ордер [{0}]. Итерация [{1}]. Бронирование слотов ресурсов", order.id, iteration));

            if (!checkOrderRules(order)) {
                throw new ServiceException(String.Format("Wrong order status"));
            }

            List<ResourceSlot> heldSlots = new List<ResourceSlot>();
            CustomerScheduleItem customerScheduleItem = new CustomerScheduleItem();

            customerScheduleItem = fetchCustomerScheduleIterationItem(order, iteration);

            if (customerScheduleItem != null) {
                return customerScheduleItem;
            }

            Service service = SEntities.Services().select(order.service_id);

            int durationSlots = service.svc_duration_slots;

            var demandsGropus = fetchServiceDemands(order.service_id);

            if (demandsGropus.Count == 0) {
                Log.Get().put(Log.WRN, String.Format(
                    ">>> Ордер [{0}]. Сервис: [{1}] не содержит требования к ресурсам.",
                    order.id,
                    order.service_id
                ));

                return customerScheduleItem;
            }
            
            Dictionary<int, Dictionary<int, Dictionary<int, List<ResourceSlot>>>> fetchedEmptySlots =
                ServiceSlots.GetInstance().fetchSlotsFromDbByDemandsAsDictionary(order, service, fetchingParams, demandsGropus);

            foreach (KeyValuePair<int, Dictionary<int, List<int>>> categoryDemands in demandsGropus)
            {
                bool altMated = false;
                foreach (KeyValuePair<int, List<int>> demandsGroup in categoryDemands.Value)
                {
                    Log.Get().put(Log.TRACE, String.Format("[{0}]", demandsGroup.Key));

                    foreach (var slots in fetchedEmptySlots[categoryDemands.Key][demandsGroup.Key])
                    {
                        if (slots.Value.Count == service.svc_duration_slots) {
                            orderingSlots(order, iteration, service, slots.Value, heldSlots, fetchingParams.orderAllSlotsOrdinals);

                            altMated = true;
                        }
                        
                        if (altMated) break;
                    }
                }
            }

            customerScheduleItem = generateIterationCustomerSchedule(order, iteration, heldSlots, updateDatabase);
            
            if (customerScheduleItem == null) {
                throw new ServiceException(
                    String.Format("Сервис [{0}]. Пустое расписание клиента.", service.id)
                );
            }

            Log.Get().put(Log.DEBUG, String.Format(">>> Ордер [{0}]. Итерация [{1}]. Бронирование слотов ресурсов", order.id, iteration));

            if (updateDatabase && heldSlots.Count > 0) {
                Log.Get().put(Log.TRACE, String.Format("Обновление БД. [heldSlots.Count]:[{0}]", heldSlots.Count));
                SEntities.Slots(true).updateList(heldSlots);
            }

            return customerScheduleItem;
        }

        public Order releaseOrderSchedule(Order order)
        {
            if (order.is_closed ?? false)
            {
                string errorMessage = String.Format(
                    "Ордер: [{0}] закрыт",
                    order.id
                );

                Log.Get().put(Log.WRN, errorMessage);

                throw new ServiceException(errorMessage);
            }

            var customerSchedule = SEntities.CustomersSchedules(true).selectList(
                $"order_id={order.id}",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            foreach (CustomerScheduleItem item in customerSchedule) {
                if (item.is_executed) {
                    string errorMessage = String.Format(
                        "Ордер: [{0}]. Итерация [{1}] выполнена!!",
                        order.id,
                        item.iteration
                    );
                    
                    Log.Get().put(Log.WRN, errorMessage);

                    throw new ServiceException(errorMessage);
                }

                if (item.is_published) {
                    string errorMessage = String.Format(
                        "Ордер: [{0}]. Итерация [{1}] Расписание выдано клиенту [{2}]!",
                        order.id,
                        item.iteration,
                        item.customer_id
                    );
                    
                    Log.Get().put(Log.WRN, errorMessage);

                    throw new ServiceException(errorMessage);
                }
            }

            releaseSlotsList(
                SEntities.Slots().selectList(
                    $"order_id={order.id}",
                    new List<string>() { "*" },
                    new List<string>() { "id" }
                ),
                true
            );

            SEntities.CustomersSchedules(true).deleteList($"order_id={order.id}");

            order.is_planned = false;
            order.is_partially_planned = false;
            
            SEntities.Orders(true).update(order);

            return order;
        }

        public Order releaseOrderIterationSchedule(Order order, int iteration)
        {
            string errorMessage = null;

            if (order.is_closed ?? false)
            {
                errorMessage = String.Format(
                    "Ордер: [{0}] закрыт",
                    order.id
                );

                Log.Get().put(Log.WRN, errorMessage);

                throw new ServiceException(errorMessage);
            }

            var customerSchedule = SEntities.CustomersSchedules(true).selectList(
                $"order_id={order.id} AND iteration={iteration}",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );
            
            if (errorMessage == null && customerSchedule.Count < 1) {
                errorMessage = String.Format(
                    "Ордер: [{0}]. Итерация: [{1}]. Пустое расписание!!",
                    order.id,
                    iteration
                );
            }
            
            var item = customerSchedule.FirstOrDefault();
            
            if (errorMessage == null && item.is_executed) {
                errorMessage = String.Format(
                    "Ордер: [{0}]. Итерация [{1}] выполнена!",
                    order.id,
                    item.iteration
                );
            }

            if (errorMessage == null && item.is_published && !item.is_skipped) {
                errorMessage = String.Format(
                    "Ордер: [{0}]. Итерация: [{1}]. Выдано клиенту: [{2}], не отмечен пропуск иитерации!",
                    order.id,
                    item.iteration,
                    item.customer_id
                );
            }

            if (errorMessage != null) {
                Log.Get().put(Log.WRN, errorMessage);

                throw new ServiceException(errorMessage);
            }

            releaseSlotsList(
                SEntities.Slots().selectList(
                    $"order_id={order.id} AND iteration={item.iteration} AND slot_start_time >= '{item.start_time.ToString("yyyyMMdd HH:mm:ss")}' AND slot_end_time <= '{item.end_time.ToString("yyyyMMdd HH:mm:ss")}'",
                    new List<string>() { "*" },
                    new List<string>() { "id" }
                ),
                true
            );

            item.is_skipped = true;
            
            SEntities.CustomersSchedules(true).update(item);

            order.is_partially_planned = true;

            SEntities.Orders(true).update(order);

            return order;
        }

        public List<ResourceSlot> releaseSlotsList(List<ResourceSlot> slots, bool updateDatabase = false)
        {
            Log.Get().put(Log.DEBUG, String.Format("<<< Освобождение списка слотов [slots.Count]:[{0}]", slots.Count));
            for (int index = 0; index < slots.Count(); index++)
            {

                ResourceSlot slot = slots[index];

                slot.order_id    = null;
                slot.iteration   = null;
                slot.customer_id = null;
            }

            if (updateDatabase)
            {
                Log.Get().put(Log.TRACE, String.Format("Обновление БД. [slots.Count]:[{0}]", slots.Count));
                SEntities.Slots(true).updateList(slots);
            }

            Log.Get().put(Log.DEBUG, String.Format(">>> Освобождение списка слотов [slots.Count]:[{0}]", slots.Count));
            
            return slots;
        }

        public Order registerExecutedOrderIteration(Order order, int? completedEmployeeId = null, int iteration = 0)
        {
            Log.Get().put(Log.TRACE, String.Format("<<< Отметка о выполнении. Ордер [{0}]. Итерация: [{1}].", order.id, iteration));

            if (order.is_closed ?? false)
            {
                string errorMessage = String.Format(
                    "Ордер: [{0}] закрыт",
                    order.id
                );

                Log.Get().put(Log.WRN, errorMessage);

                throw new ServiceException(errorMessage);
            }


            DateTime executeAt = DateTime.Now;

            List<CustomerScheduleItem> customerSchedule = SEntities.CustomersSchedules().selectList(
                $"order_id={order.id} AND iteration={iteration}  AND is_skipped=0 AND ISNULL(moved_to,0)=0",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            if (customerSchedule.Count > 0) {
                Log.Get().put(Log.DEBUG, String.Format("Обработка расписания клиента. Ордер [{0}]. Итерация: [{1}]. ", order.id, iteration));

                foreach (CustomerScheduleItem item in customerSchedule)
                {
                    item.is_executed = true;
                    item.completed_employee_id = completedEmployeeId;
                    executeAt = item.end_time;
                }

                SEntities.CustomersSchedules(true).updateList(customerSchedule);
            }

            order.repeated_count += 1;

            if (order.repeated_count >= order.repeats_count)
            {
                Log.Get().put(Log.DEBUG, String.Format("Полное закрытие ордера. Ордер [{0}]. Итерация: [{1}]. Повторов: [{2}].", order.id, iteration, order.repeated_count));
                order.executed_at = executeAt;
                order.is_closed = true;             
            }

            SEntities.Orders(true).update(order);

            return order;
        }
        
        public Order unregisterExecutedOrderIteration(Order order, int iteration = 0)
        {
            Log.Get().put(Log.TRACE, String.Format("<<< Отметка о выполнении. Ордер [{0}]. Итерация: [{1}].", order.id, iteration));

            if (order.is_closed ?? false)
            {
                string errorMessage = String.Format(
                    "Ордер: [{0}] закрыт",
                    order.id
                );

                Log.Get().put(Log.WRN, errorMessage);

                throw new ServiceException(errorMessage);
            }


            DateTime? executeAt = DateTime.Now;

            List<CustomerScheduleItem> customerSchedule = SEntities.CustomersSchedules().selectList(
                $"order_id={order.id} AND iteration={iteration}  AND is_skipped=0 AND ISNULL(moved_to,0)=0",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            if (customerSchedule.Count > 0) {
                Log.Get().put(Log.DEBUG, String.Format("Обработка рисписания клиента. Ордер [{0}]. Итерация: [{1}]. ", order.id, iteration));

                foreach (CustomerScheduleItem item in customerSchedule)
                {
                    item.is_executed = false;
                    item.completed_employee_id = null;
                    executeAt = null;
                }

                SEntities.CustomersSchedules(true).updateList(customerSchedule);
            }

            if (order.repeated_count > 0)
            {
                Log.Get().put(Log.DEBUG, String.Format("Отмена выполнения итерации ордера. Ордер [{0}]. Итерация: [{1}]. Повторов отмечено: [{2}].", order.id, iteration, order.repeated_count));
                order.repeated_count -= 1;
                order.executed_at = null;
                SEntities.Orders(true).update(order);
            }

            return order;
        }

        private bool orderingSlots(
            Order order,
            int iteration,
            Service service,
            List<ResourceSlot> mathcedSlots,
            List<ResourceSlot> heldSlots,
            bool orderAllSlotsOrdinals
        ) {
            Log.Get().put(Log.TRACE, String.Format("<<< Ордер [{0}]. Итерация: [{1}]. Обработка подходящих слотов: [{2}].", order.id, iteration, mathcedSlots.Count));

            for (int slotIndex = 0; slotIndex < mathcedSlots.Count; slotIndex++)
            {
                ResourceSlot slot = mathcedSlots[slotIndex];
                slot.order_id = order.id;
                slot.iteration = iteration;
                slot.customer_id = order.customer_id;

                heldSlots.Add(slot);
            }

            if (orderAllSlotsOrdinals) {
                captureAllOrdinalSlotsInPeriod(mathcedSlots, heldSlots);
            }

            Log.Get().put(Log.TRACE, String.Format(">>> Ордер [{0}]. Итерация: [{1}]. Обработано слотов: [{2}].", order.id, iteration, mathcedSlots.Count));

            return true;
        }

        private List<ResourceSlot> captureAllOrdinalSlotsInPeriod(
            List<ResourceSlot> slots,
            List<ResourceSlot> heldSlots
        ) {
            foreach (ResourceSlot existedSlot in slots)
            {
                if (existedSlot.resource_category_id == EMPLOYEE_CATEGORY) {
                    var ordinalSlots = SEntities.Slots().selectList(
                        $"resource_category_id={existedSlot.resource_category_id} AND resource_general_id={existedSlot.resource_general_id} AND slot_start_time='{existedSlot.slot_start_time}' AND slot_end_time='{existedSlot.slot_end_time}' AND slot_ordinal<>{existedSlot.slot_ordinal}",
                        new List<string>() { "*" },
                        new List<string>() { "slot_ordinal" }
                    );

                    for (int slotIndex = 0; slotIndex < ordinalSlots.Count; slotIndex++)
                    {
                        ResourceSlot slot = ordinalSlots[slotIndex];
                        slot.order_id = existedSlot.order_id;
                        slot.iteration = existedSlot.iteration;
                        slot.customer_id = existedSlot.customer_id;

                        heldSlots.Add(slot);
                    }
                }
            }

            return heldSlots;
        }

        private CustomerScheduleItem fetchCustomerScheduleIterationItem(Order order, int iteration)
        {
            Log.Get().put(Log.TRACE, String.Format("<<< Ордер [{0}]. Итерация: [{1}]. Получение ранее назначенной единицы расписания клиента.", order.id, iteration));

            List<CustomerScheduleItem> customerSchedule = SEntities.CustomersSchedules().selectList(
                $"order_id={order.id} AND iteration={iteration} AND is_skipped=0 AND ISNULL(moved_to,0)=0",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            if (customerSchedule.Count > 0) {
                Log.Get().put(Log.TRACE, String.Format(">>> Ордер [{0}]. Итерация: [{1}]. Возврат ранее назначенной единицы расписания клиента.", order.id, iteration));
                return customerSchedule.FirstOrDefault();
            }

            return default(CustomerScheduleItem);
        }

        private DateTime getAlignedDateTimeByPreferredTime(DateTime timeToAlign, DateTime preferredTime)
        {
            return new DateTime(
                timeToAlign.Year,
                timeToAlign.Month,
                timeToAlign.Day,
                preferredTime.Hour,
                preferredTime.Minute,
                preferredTime.Second
            );
        }

        private Dictionary<int, Dictionary<int, List<int>>> fetchServiceDemands(int serviceId)
        {
            Log.Get().put(Log.TRACE, String.Format("<<< Старт получения требований сервиса id: [{0}]", serviceId));
            
            Dictionary<int, Dictionary<int, List<int>>> demandsDict = new Dictionary<int, Dictionary<int, List<int>>>();
            
            List<ServicePlaceDemandItem> placesDemands = SEntities.DemandsServices<ServicePlaceDemandItem>().selectList(
                $"service_id={serviceId}",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            List<ServiceEmployeeDemandItem> employesDemands = SEntities.DemandsServices<ServiceEmployeeDemandItem>().selectList(
                $"service_id={serviceId}",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            List<ServiceEquipmentDemandItem> eqpDemands = SEntities.DemandsServices<ServiceEquipmentDemandItem>().selectList(
                $"service_id={serviceId}",
                new List<string>() { "*" },
                new List<string>() { "id" }
            );

            if (placesDemands.Count == 0 && employesDemands.Count == 0 && eqpDemands.Count == 0) {
                Log.Get().put(Log.TRACE, String.Format("Требования пусты. Возврат нулевого результата."));
                return demandsDict;
            }

            if (placesDemands.Count > 0) {
                var groupedPlacesDemands =
                    from demand in placesDemands
                    group demand by demand.alt_group_id into groupDemands
                    orderby groupDemands.Key
                    select groupDemands;


                demandsDict[PLACE_CATEGORY] = new Dictionary<int, List<int>>();

                foreach (var groupDemands in groupedPlacesDemands)
                {
                    foreach (var demand in groupDemands)
                    {
                        if (demandsDict[PLACE_CATEGORY].ContainsKey(demand.alt_group_id ?? 0))
                        {
                            var existedDemandsList = demandsDict[PLACE_CATEGORY][demand.alt_group_id ?? 0];
                            existedDemandsList.Add(demand.resource_general_id ?? 0);
                            demandsDict[PLACE_CATEGORY][demand.alt_group_id ?? 0] = existedDemandsList;

                            continue;
                        }

                        List<int> demandsList = new List<int>();
                        demandsList.Add(demand.resource_general_id ?? 0);
                        demandsDict[PLACE_CATEGORY].Add(demand.alt_group_id ?? 0, demandsList);
                    }
                }
            }

            if (employesDemands.Count > 0) {
                var groupedEmployesDemands =
                    from demand in employesDemands
                    group demand by demand.alt_group_id into groupDemands
                    orderby groupDemands.Key
                    select groupDemands;

                demandsDict[EMPLOYEE_CATEGORY] = new Dictionary<int, List<int>>();

                foreach (var groupDemands in groupedEmployesDemands)
                {
                    foreach (var demand in groupDemands)
                    {
                        if (demandsDict[EMPLOYEE_CATEGORY].ContainsKey(demand.alt_group_id ?? 0))
                        {
                            var existedDemandsList = demandsDict[EMPLOYEE_CATEGORY][demand.alt_group_id ?? 0];
                            existedDemandsList.Add(demand.resource_general_id ?? 0);
                            demandsDict[EMPLOYEE_CATEGORY][demand.alt_group_id ?? 0] = existedDemandsList;

                            continue;
                        }

                        List<int> demandsList = new List<int>();
                        demandsList.Add(demand.resource_general_id ?? 0);
                        demandsDict[EMPLOYEE_CATEGORY].Add(demand.alt_group_id ?? 0, demandsList);
                    }
                }
            }

            if (eqpDemands.Count > 0) {
                var groupedEqpDemands =
                    from demand in eqpDemands
                    group demand by demand.alt_group_id into groupDemands
                    orderby groupDemands.Key
                    select groupDemands;


                demandsDict[EQUIPMENT_CATEGORY] = new Dictionary<int, List<int>>();

                foreach (var groupDemands in groupedEqpDemands)
                {
                    foreach (var demand in groupDemands)
                    {
                        if (demandsDict[EQUIPMENT_CATEGORY].ContainsKey(demand.alt_group_id ?? 0))
                        {
                            var existedDemandsList = demandsDict[EQUIPMENT_CATEGORY][demand.alt_group_id ?? 0];
                            existedDemandsList.Add(demand.resource_general_id ?? 0);
                            demandsDict[EQUIPMENT_CATEGORY][demand.alt_group_id ?? 0] = existedDemandsList;

                            continue;
                        }

                        List<int> demandsList = new List<int>();
                        demandsList.Add(demand.resource_general_id ?? 0);
                        demandsDict[EQUIPMENT_CATEGORY].Add(demand.alt_group_id ?? 0, demandsList);
                    }
                }
            }
            
            Log.Get().put(Log.TRACE, String.Format(">>> Успешный возврат сгруппированных треобваний"));

            return demandsDict;
        }

        private CustomerScheduleItem generateIterationCustomerSchedule(Order order, int iteration, List<ResourceSlot> slots, bool updateDatabase = false)
        {
            Log.Get().put(Log.INF, String.Format("<<< Ордер [{0}]. Генерация расписания клиента по ордеру и слотам", order.id));

            CustomerScheduleItem customerScheduleItem = fetchCustomerScheduleIterationItem(order, iteration);
            if (customerScheduleItem != null) {
                return customerScheduleItem;
            }

            if (slots.Count == 0) {
                return default(CustomerScheduleItem);
            }
            
            var groupedSlots =
                from slot in slots
                group new { slot.slot_start_time, slot.slot_end_time, slot.order_id, slot.iteration } by slot.slot_start_time.ToString("yyyyMMdd") into dayGroup
                orderby dayGroup.Key
                select dayGroup;

            var employeeId =
                from slot in slots
                where slot.resource_category_id == EMPLOYEE_CATEGORY
                orderby slot.resource_general_id
                select slot.resource_general_id;

            var placeId =
                from slot in slots
                where slot.resource_category_id == ServiceSessions.PLACE_CATEGORY
                orderby slot.resource_general_id
                select slot.resource_general_id;

            var scheduleItem = new CustomerScheduleItem();

            scheduleItem.customer_id         = order.customer_id;
            scheduleItem.order_id            = order.id;
            scheduleItem.iteration           = iteration;
            scheduleItem.service_id          = order.service_id;
            scheduleItem.csession_id         = order.csession_id;
            scheduleItem.start_time          = groupedSlots.FirstOrDefault().Min(x => x.slot_start_time);
            scheduleItem.end_time            = groupedSlots.FirstOrDefault().Max(x => x.slot_end_time);
            scheduleItem.schedule_date       = scheduleItem.start_time;
            scheduleItem.place_general_id    = placeId.FirstOrDefault();
            scheduleItem.employee_general_id = employeeId.FirstOrDefault();
                            
            
            if (updateDatabase) {
                Log.Get().put(Log.INF, String.Format("Ордер [{0}]. Обновление/запись расписания клиента в БД", order.id));

                SEntities.CustomersSchedules(true).insert(scheduleItem);
            }

            Log.Get().put(Log.INF, String.Format(">>> Ордер [{0}]. Успешный возврат расписания клиента.", order.id));

            return scheduleItem;
        }

        private bool checkOrderRules(Order order)
        {
            // if ((order.is_planned ?? false) && !(order.is_partially_planned ?? false))
            // {
            //     string errorMessage = String.Format(
            //         "Ордер: [{0}] запланирован ранее",
            //         order.id
            //     );

            //     Log.Get().put(Log.WRN, errorMessage);

            //     return false;
            // }

            if (order.is_closed ?? false)
            {
                string errorMessage = String.Format(
                    "Ордер: [{0}] закрыт",
                    order.id
                );

                Log.Get().put(Log.WRN, errorMessage);

                return false;
            }

            if (!(order.is_approved ?? false))
            {
                string errorMessage = String.Format(
                    "Ордер: [{0}] требует подтверждения",
                    order.id
                );

                Log.Get().put(Log.WRN, errorMessage);

                return false;
            }

            return true;
        }

        private bool checkScheduleRules(Order order)
        {
            return true;
        }
    }
}
