using System.Collections.Generic ;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumEntitiesMeta<ModelType>
    where ModelType : class
    {
        int checkRowsCount(string filter = "");
        int getPagesCount(string filter = "");
    }
}
