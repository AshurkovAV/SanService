using System;

namespace SanatoriumEntities.Models.Services
{
    public class GeneralPlaceItem : BaseModel
    {
        public string       pgl_type               { get; set; }
        public string       pgl_name               { get; set; }
        public string       pgl_ordinal            { get; set; }
        public int?         pgl_required_capacity  { get; set; } = 1;
        public int?         pgl_required_square    { get; set; }
        public override string getDatabaseEntityName()
        {
            return "places_general_list";
        }
    }
}
