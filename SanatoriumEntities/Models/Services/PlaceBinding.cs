using System;

namespace SanatoriumEntities.Models.Services
{
    public class PlaceBinding : AbstractResourceBinding
    {
        public int?       place_general_id     { get; set; }
        public int?       place_id             { get; set; }
        public override string getDatabaseEntityName()
        {
            return "places_bindings";
        }
    }
}
