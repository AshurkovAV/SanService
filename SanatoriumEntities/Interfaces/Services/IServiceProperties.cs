namespace SanatoriumEntities.Interfaces.Services
{
    public enum ServiceProperties : uint
    {
        NONE        = 0b000000000000000,

        GROUP           = 1,//Групповая
        AUX             = 2,//Служебная
        RESERVE_4       = 4,//Резерв
        CAT_SPECIAL         = 8,//Специальная отметка

        CAT_HEAT_16         = 16,//Тепло
        CAT_ELECTRO_32      = 32,//Электро
        CAT_WATER_64        = 64,//Водная
        CAT_MASSAGE_128     = 128,//Массаж

        CAT_PHYS_256        = 256,//Физио
        CAT_LFK_512         = 512,//ЛФК
        CAT_POOL_SAMLL_1024 = 1024,//Маленький бассейн
        CAT_POOL_2048   = 2048,//Бассейн

        FOR_ADULT       = 4096,//Взрослые
        FOR_CHILD       = 8192,//Дети
        FOR_MALE        = 16384,//Мужчины
        FOR_FEMALE      = 32768,//Женщины

        CAT_EXARTA_65536    = 65536,//Экзарта
        CAT_MECH_131072     = 131072,//Механо
        RESERVE_262144      = 262144,//Резерв
        RESERVE_524288      = 524288,//Резерв

        ALL = ~NONE
    }

    public interface IServiceProperties
    {
        bool hasProperty(ServiceProperties flag);
        ServiceProperties setProperty(ServiceProperties flag, bool? value);
    }
}
