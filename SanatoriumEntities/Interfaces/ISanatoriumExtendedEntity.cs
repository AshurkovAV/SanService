using System.Collections.Generic;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumExtendedEntity<ModelType> :
        ISanatoriumEntityCRUD<ModelType>,
        ISanatoriumEntitiesListCRUD<ModelType>,
        ISanatoriumEntitiesMeta<ModelType>,
        ISanatoriumEntitiesDictRead<ModelType>
    where ModelType : class
    {
    }
}
