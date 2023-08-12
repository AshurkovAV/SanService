using System;
using System.Collections.Generic;
using SanatoriumEntities.Models.Session;
using SanatoriumEntities.Models.Program;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Entities.Overriden;
using SanatoriumEntities.Interfaces;
using SanatoriumEntities.Interfaces.Services;
using SanatoriumEntities.Exceptions;

namespace SanatoriumEntities.ServicesClasses
{
    public class ServiceOrders:
        SimpleEntity<Order>,
        IOrders
    {
        private ServiceOrders() { }
        private static ServiceOrders _instance;
        private static readonly object _lock = new object();
        public static ServiceOrders GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServiceOrders();
                    }
                }
            }
            return _instance;
        }

        public List<Order> generateOrdersList(ProgramAppointment programAppointment, bool updateDatabase = false)
        {
            if (SEntities.AppointedPrograms(true).select(programAppointment.id ?? 0).has_orders)
            {
                string errorMessage = String.Format(
                    "Назначение программы: [{0}], программа: [{1}]. Ордеры назначены.",
                    programAppointment.id,
                    programAppointment.program_id
                );
                
                throw new ServiceException(errorMessage);
            }
            
            List<Order> orders = new List<Order>{};

            Session cSession = SEntities.Sessions().select(programAppointment.csession_id);

            List<ProgramStructItem> programStruct = SEntities.ProgramsStructs().selectList($"program_id={programAppointment.program_id}");
            
            foreach (ProgramStructItem programItem in programStruct)
            {
                Order order     = new Order();
                Service service = SEntities.Services().select(programItem.service_id);

                order.program_id             = programItem.program_id;
                order.program_appointment_id = programAppointment.id;
                order.employee_general_id    = programAppointment.employee_general_id;
                order.csession_id            = programAppointment.csession_id;
                order.contract_id            = programAppointment.contract_id;

                order.service_id         = programItem.service_id;
                order.customer_id        = cSession.customer_id;
                order.repeats_count      = programItem.repeats_count;
                order.repeat_period_days = programItem.repeats_freq;
                order.is_approved        = programItem.alt_group_id > 0 ? false : true;
                order.cost_rub_minor     = programItem.sps_cost_rub_minor * programItem.repeats_count;

                orders.Add(order);
            }

            if (updateDatabase)
            {
                SEntities.Orders(true).insertList(orders);
                programAppointment.has_orders = true;
                SEntities.AppointedPrograms(true).update(programAppointment);
            }

            return orders;
        }
    }
}
