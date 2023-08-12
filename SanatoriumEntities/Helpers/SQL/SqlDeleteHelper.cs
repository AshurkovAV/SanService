using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Helpers.SQL
{
    public class SqlDeleteHelper<ModelType> : SqlAbstractHelper<ModelType>
    where ModelType : BaseModel
    {
        protected readonly List<string> doNotUpdateFields = new List<string>() {ID_FIELD, STATUS, CREATED_AT, CREATED_BY};

        public SqlDeleteHelper(string tableName, string username) : base(tableName, username) { }

        public string generateQuery(int id, bool isRestore = false, bool isHardDelete = false)
        {
            string resultStatus = isRestore ? "1" : "0";
            string conditionStatus = isRestore ? "0" : "1";

            if (isHardDelete && !isRestore) {
                return $"DELETE FROM {this.table}"
                + $" WHERE {ID_FIELD}={id}"
                + $" AND {STATUS}={conditionStatus}";
            }

            return $"UPDATE {this.table}"
                + $" SET {STATUS}={resultStatus}, {UPDATED_AT}=getdate(), {UPDATED_BY}='{this.user}'"
                + $" WHERE {ID_FIELD}={id}"
                + $" AND {STATUS}={conditionStatus}";
            ;
        }

        public string generateQuery(List<int> ids, bool isRestore = false, bool isHardDelete = false)
        {
            string resultStatus = isRestore ? "1" : "0";
            string conditionStatus = isRestore ? "0" : "1";

            if (isHardDelete && !isRestore) {
                return $"DELETE FROM {this.table}"
                    + $" WHERE {ID_FIELD} IN ({String.Join(",", ids)})"
                    + $" AND {STATUS}={conditionStatus}";
            }

            return $"UPDATE {this.table}"
                + $" SET {STATUS}={resultStatus}, {UPDATED_AT}=getdate(), {UPDATED_BY}='{this.user}'"
                + $" WHERE {ID_FIELD} IN ({String.Join(",", ids)})"
                + $" AND {STATUS}={conditionStatus}";
            ;
        }
    }
}
