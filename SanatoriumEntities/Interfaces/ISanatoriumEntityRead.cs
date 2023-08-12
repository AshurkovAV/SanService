using System.Collections.Generic;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumEntityRead<ModelType>
    where ModelType : class
    {
        ModelType select(int id);
    }
}
