using System.Collections.Generic;
using SanatoriumEntities.Models;
using SanatoriumEntities.Models.Session;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Entities.Overriden;
using SanatoriumEntities.Interfaces;

namespace SanatoriumEntities.ServicesClasses
{
    public class SEntities
    {
        private static SEntities _instance;
        private static readonly object _lock = new object();
        public static SEntities Get()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SEntities();
                    }
                }
            }
            return _instance;
        }

        private static Dictionary<string, dynamic> entitiesCollection = new Dictionary<string, dynamic>();
        private static Dictionary<string, bool> entitiesCollectionBlocks = new Dictionary<string, bool>();

        public static ISanatoriumSimpleEntity<Model> GetEntity<Model>(bool getStaticInstance = false)
            where Model: BaseModel, new()
        {
            string typeName = typeof(Model).ToString();

            if (!entitiesCollection.ContainsKey(typeName)) {
                dynamic instance = EntityBuilder.getSimpleEntity<Model>();
                entitiesCollection.Add(typeName, instance);
                
                entitiesCollectionBlocks.Add(typeName, true);
            }

            if (entitiesCollectionBlocks.ContainsKey(typeName)) {
                if (getStaticInstance) {
                    return entitiesCollection[typeName];
                }
            }

            return EntityBuilder.getSimpleEntity<Model>();
        }

        public static ISanatoriumSimpleEntity<ResourceSlot> Slots(bool getStaticInstance = false)
        {
            return GetEntity<ResourceSlot>(getStaticInstance);
        }
        
        public static ISanatoriumSimpleEntity<GeneralEmployeeItem> GeneralEmployes(bool getStaticInstance = false)
        {
            return GetEntity<GeneralEmployeeItem>(getStaticInstance);
        }
        public static ISanatoriumSimpleEntity<GeneralPlaceItem> GeneralPlaces(bool getStaticInstance = false)
        {
            return GetEntity<GeneralPlaceItem>(getStaticInstance);
        }

        public static ISanatoriumSimpleEntity<GeneralEquipmentItem> GeneralEquipment(bool getStaticInstance = false)
        {
            return GetEntity<GeneralEquipmentItem>(getStaticInstance);
        }

        public static ISanatoriumSimpleEntity<ResourceSlot> ResourcesSlots(bool getStaticInstance = false)
        {
            return GetEntity<ResourceSlot>(getStaticInstance);
        }

        public static ISanatoriumSimpleEntity<ServiceDemandItem> DemandsServices<ServiceDemandItem>(bool getStaticInstance = false)
            where ServiceDemandItem: AbstractServiceDemandItem, new()
        {
            return GetEntity<ServiceDemandItem>(getStaticInstance);
        }

        public static ISanatoriumSimpleEntity<Order> Orders(bool getStaticInstance = false)
        {
            return GetEntity<Order>(getStaticInstance);
        }
        public static ISanatoriumSimpleEntity<Service> Services(bool getStaticInstance = false)
        {
            return GetEntity<Service>(getStaticInstance);
        }
        public static ISanatoriumSimpleEntity<Session> Sessions(bool getStaticInstance = false)
        {
            return GetEntity<Session>(getStaticInstance);
        }

        public static ISanatoriumSimpleEntity<ProgramAppointment> AppointedPrograms(bool getStaticInstance = false)
        {
            return GetEntity<ProgramAppointment>(getStaticInstance);
        }

        public static ISanatoriumSimpleEntity<CustomerScheduleItem> CustomersSchedules(bool getStaticInstance = false)
        {
            return GetEntity<CustomerScheduleItem>(getStaticInstance);
        }
        public static ProgramsStructs ProgramsStructs(bool getStaticInstance = false)
        {
            string typeName = typeof(ProgramsStructs).ToString();

            if (!entitiesCollection.ContainsKey(typeName)) {
                dynamic instance = new ProgramsStructs();
                entitiesCollection.Add(typeName, instance);
            }
            
            if (getStaticInstance) {
                return entitiesCollection[typeName];
            }

            return new ProgramsStructs();
        }
    }
}
