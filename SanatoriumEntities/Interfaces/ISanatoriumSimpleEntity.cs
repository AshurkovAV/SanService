using System.Collections.Generic;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumSimpleEntity<ModelType> :
        ISanatoriumEntityCRUD<ModelType>,
        ISanatoriumEntitiesListCRUD<ModelType>,
        ISanatoriumEntitiesMeta<ModelType>
    where ModelType : class
    {
    }
}
