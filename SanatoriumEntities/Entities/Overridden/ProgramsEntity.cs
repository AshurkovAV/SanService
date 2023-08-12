using SanatoriumEntities.Models.Program;

namespace SanatoriumEntities.Entities.Overriden
{
    public class ProgramsEntity : SimpleEntity<Program> 
    {
        public override int delete(int id, bool isHardDelete = false)
        {
            ProgramsStructs programsStructs = new ProgramsStructs();
            programsStructs.deleteList($"program_id={id}", isHardDelete);

            return base.delete(id, isHardDelete);
        }
    }
}
