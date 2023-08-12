using System;

namespace SanatoriumEntities.Models.Services
{
    public class PlaceResource : BaseModel
    {
        public string       place_name              { get; set; }
        public string       place_number            { get; set; }
        public int?         place_floor             { get; set; } = 1;
        public int?         place_building          { get; set; } = 1;
        public int?         place_capacity_persons  { get; set; }
        public string       place_data_json         { get; set; } = "{}";
        public DateTime?    place_dasabled_before   { get; set; }
        public override string getDatabaseEntityName()
        {
            return "places";
        }
    }
}
