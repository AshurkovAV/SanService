using System.Collections.Generic ;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Interfaces
{
    public interface ISanatoriumEntitiesListRead<ModelType>
    where ModelType : class
    {
        int setRowsPerPageCount(int rowsPerPage);
        int getRowsPerPageCount();
        List<ModelType> selectList(string filter, string orderByFields);
        List<ModelType> selectList(string filter, List<string> fields, List<string> orderByFields);
        List<ModelType> selectList(List<int> ids);
    }
}
