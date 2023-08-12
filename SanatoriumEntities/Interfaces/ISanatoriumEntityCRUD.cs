using System.Collections.Generic ;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumEntityCRUD<ModelType>:
        ISanatoriumEntityRead<ModelType>
    where ModelType : class
    {
        int insert(ModelType model);
        int update(ModelType model);
        int delete(int id, bool isHardDelete = false);
        int restore(int id);
        ModelType insertAndSelect(ModelType model);
        ModelType updateAndSelect(ModelType model);
        ModelType deleteAndSelect(int id);
        ModelType restoreAndSelect(int id);
    }
}
