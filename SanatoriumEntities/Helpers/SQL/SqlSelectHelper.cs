using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Helpers.SQL
{
    public class SqlSelectHelper<ModelType> : SqlAbstractHelper<ModelType>
    where ModelType : BaseModel
    {
        public SqlSelectHelper(string tableName, string username) : base(tableName, username) { }
        
        public string generateQuery(int id)
        {
            return getSelectSQLStatement(new List<string>() {"*"}) + getWhereSQLStatement($"{ID_FIELD} = {id}");
        }

        public string generateQuery(string filter = "", string orderByFields = "id")
        {
            return getSelectSQLStatement(new List<string>() {"*"})
                + getWhereSQLStatement(filter)
                + getOrderBySQLStatement(orderByFields, selectableRowsCount, selectablePage)
            ;
        }
        
        public string generateQuery(string filter, List<string> fields, List<string> orderByFields)
        {
            return getSelectSQLStatement(fields)
                + getWhereSQLStatement(filter)
                + getOrderBySQLStatement(orderByFields, selectableRowsCount, selectablePage)
            ;
        }

        public string generateQuery(bool isNeedRowsCount, string filter = "")
        {
            return getSelectSQLStatement(new List<string>() {$"COUNT({ID_FIELD}) AS COUNT"})
                + getWhereSQLStatement(filter)
            ;
        }

        public string generateQuery(List<int> ids)
        {
            return getSelectSQLStatement(new List<string>() {"*"})
                + getWhereSQLStatement($"{ID_FIELD} IN ({String.Join(",", ids)})")
                + getOrderBySQLStatement(new List<string>() {"id"}, selectableRowsCount, selectablePage)
            ;
        }

        private string getSelectSQLStatement(List<string> fields)
        {
            return $"SELECT {String.Join(",", fields)} FROM {this.table}";
        }
    }
}