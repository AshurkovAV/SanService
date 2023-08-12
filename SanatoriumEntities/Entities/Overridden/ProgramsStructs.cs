using System.Collections.Generic;
using SanatoriumEntities.Models.Program;
using SanatoriumEntities.Interfaces;

namespace SanatoriumEntities.Entities.Overriden
{
    public class ProgramsStructs : SimpleEntity<ProgramStructItem>, ISanatoriumExtendedEntity<ProgramStructItem>
    {
        public Dictionary<int, ProgramStructItem> selectDictionary(int selectId)
        {
            Dictionary<int, ProgramStructItem> programStruct = new Dictionary<int, ProgramStructItem>();

            List<ProgramStructItem> programStructList = selectList($"program_id={selectId}");

            foreach (ProgramStructItem item in programStructList)
            {
                programStruct.Add(item.service_id, item);
            }

            return programStruct;
        }
    }
}
