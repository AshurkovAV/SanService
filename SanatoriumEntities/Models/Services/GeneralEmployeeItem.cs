using System;

namespace SanatoriumEntities.Models.Services
{
    public class GeneralEmployeeItem : BaseModel
    {
        public string       egl_rank                { get; set; }
        public string       egl_name                { get; set; }
        public string       egl_department          { get; set; }
        public string       egl_ordinal             { get; set; }
        public int?         egl_size_hours_day      { get; set; } = 8;
        public int?         position_catalog_id     { get; set; }
        public int?         place_general_id        { get; set; }
        public bool?        egl_is_vacant           { get; set; } = false;
        public override string getDatabaseEntityName()
        {
            return "employees_general_list";
        }
    }
}
