using System.Collections.Generic;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumAggregateEntity<ModelType> :
        ISanatoriumEntityRead<ModelType>,
        ISanatoriumEntitiesListRead<ModelType>,
        ISanatoriumEntitiesMeta<ModelType>
    where ModelType : class
    {
    }
}
