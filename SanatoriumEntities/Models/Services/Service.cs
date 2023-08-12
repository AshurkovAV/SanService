using System;
using SanatoriumEntities.Interfaces.Services;
using SanatoriumEntities.Entities;
namespace SanatoriumEntities.Models.Services
{
    public static class GlobalConst
    {
        public const string SKIP_IN_SQL = "skip_in_sql_query";
    }
    [AttributeUsage(AttributeTargets.All)]
    public class ServiceAttribute : Attribute
    {
        private string attrName;
        public ServiceAttribute(string name)
        {
            attrName = name;
        }
        public string Name
        {
            get
            {
                return attrName;
            }
        }
    }

    public class Service : BaseModel, IServiceProperties
    {
        public int svc_category_id { get; set; }
        public string svc_code { get; set; }
        public string svc_name { get; set; }
        public string svc_description { get; set; }
        public string svc_specialist_profile { get; set; }
        public int svc_duration_slots { get; set; }
        public int svc_duration_min { get; set; }
        public bool? svc_break_included { get; set; }
        public int svc_max_group_size { get; set; }
        public int svc_properties { get; set; } = 0;

        /**/
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_group
        {
            get
            {
                return hasProperty(ServiceProperties.GROUP);
            }
            set
            {
                setProperty(ServiceProperties.GROUP, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_adult
        {
            get
            {
                return hasProperty(ServiceProperties.FOR_ADULT);
            }
            set
            {
                setProperty(ServiceProperties.FOR_ADULT, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_child
        {
            get
            {
                return hasProperty(ServiceProperties.FOR_CHILD);
            }
            set
            {
                setProperty(ServiceProperties.FOR_CHILD, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_for_male
        {
            get
            {
                return hasProperty(ServiceProperties.FOR_MALE);
            }
            set
            {
                setProperty(ServiceProperties.FOR_MALE, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_for_female
        {
            get
            {
                return hasProperty(ServiceProperties.FOR_FEMALE);
            }
            set
            {
                setProperty(ServiceProperties.FOR_FEMALE, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_aux
        {
            get
            {
                return hasProperty(ServiceProperties.AUX);
            }
            set
            {
                setProperty(ServiceProperties.AUX, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_heat
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_HEAT_16);
            }
            set
            {
                setProperty(ServiceProperties.CAT_HEAT_16, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_electro
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_ELECTRO_32);
            }
            set
            {
                setProperty(ServiceProperties.CAT_ELECTRO_32, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_water
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_WATER_64);
            }
            set
            {
                setProperty(ServiceProperties.CAT_WATER_64, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_massage
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_MASSAGE_128);
            }
            set
            {
                setProperty(ServiceProperties.CAT_MASSAGE_128, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_phys
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_PHYS_256);
            }
            set
            {
                setProperty(ServiceProperties.CAT_PHYS_256, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_lfk
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_LFK_512);
            }
            set
            {
                setProperty(ServiceProperties.CAT_LFK_512, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_pool_small
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_POOL_SAMLL_1024);
            }
            set
            {
                setProperty(ServiceProperties.CAT_POOL_SAMLL_1024, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_pool
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_POOL_2048);
            }
            set
            {
                setProperty(ServiceProperties.CAT_POOL_2048, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_exarta
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_EXARTA_65536);
            }
            set
            {
                setProperty(ServiceProperties.CAT_EXARTA_65536, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_mech
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_MECH_131072);
            }
            set
            {
                setProperty(ServiceProperties.CAT_MECH_131072, value);
            }
        }
        [ServiceAttribute(GlobalConst.SKIP_IN_SQL)]
        public bool? svc_is_special
        {
            get
            {
                return hasProperty(ServiceProperties.CAT_SPECIAL);
            }
            set
            {
                setProperty(ServiceProperties.CAT_SPECIAL, value);
            }
        }
        /**/

        public int? svc_default_cost_rub_minor { get; set; }

        public override string getDatabaseEntityName()
        {
            return "services_list";
        }

        private ServiceProperties serviceProperties
        {
            get
            {
                return (ServiceProperties)svc_properties;
            }
            set
            {
                svc_properties = (int)value;
                svc_category_id = svc_properties;
            }
        }

        public bool hasProperty(ServiceProperties flag)
        {
            return ((Enum)serviceProperties).HasFlag(flag);
        }

        public ServiceProperties setProperty(ServiceProperties flag, bool? value)
        {
            if (value ?? false)
            {
                serviceProperties |= flag;
            }
            else
            {
                serviceProperties &= ~flag;
            }

            return serviceProperties;
        }
    }
}
