using System.Collections.Generic ;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumEntitiesListCRUD<ModelType>:
        ISanatoriumEntitiesListRead<ModelType>
    where ModelType : class
    {
        int insertList(List<ModelType> modelsObjectsList);
        int updateList(List<ModelType> modelsObjectsList);
        int deleteList(List<int> ids, bool isHardDelete = false);
        int deleteList(string filter, bool isHardDelete = false);
    }
}
