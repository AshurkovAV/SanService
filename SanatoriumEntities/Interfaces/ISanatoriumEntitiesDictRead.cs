using System.Collections.Generic ;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumEntitiesDictRead<ModelType>
    where ModelType : class
    {
        Dictionary<int, ModelType> selectDictionary(int selectId);
    }
}
