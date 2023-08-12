using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Helpers.SQL
{
    public abstract class SqlAbstractHelper<ModelType>
    where ModelType : BaseModel
    {
        public const string ID_FIELD = "id",
            UPDATED_AT  = "updated_at",
            UPDATED_BY  = "updated_by",
            CREATED_AT  = "created_at",
            CREATED_BY  = "created_by",
            IP4         = "ip4",
            STATUS      = "active",
            USERID_FIELD = "UserId"
        ;
        public int selectablePage { get; set; } = 0;
        public int selectableRowsCount { get; set; } = 99999;
        public int selectableStatus { get; set; } = 1;

        protected readonly List<string> serviceFields     = new List<string>() {ID_FIELD, STATUS, IP4, CREATED_AT, CREATED_BY, UPDATED_AT, UPDATED_BY};
        protected string table;
        protected string user;

        public SqlAbstractHelper(string tableName, string username)
        {
            this.table = tableName;
            this.user = username;
        }

        protected string getWhereSQLStatement(string filter = "")
        {
            if (filter.Length > 0) {
                return $" WHERE {filter} AND {STATUS}={selectableStatus}";
            } else {
                return $" WHERE {STATUS}={selectableStatus}";
            }
        }

        protected string getOrderBySQLStatement(string orderByFields = "1", int rows = 1, int page = 0)
        {
            return $" ORDER BY {orderByFields} OFFSET {page * rows} ROWS FETCH NEXT {rows} ROWS ONLY";
        }

        protected string getOrderBySQLStatement(List<string> orderByFields, int rows = 1, int page = 0)
        {
            if (orderByFields.Count == 0) {
                orderByFields.Add("1");
            }

            return $" ORDER BY {String.Join(",", orderByFields)} OFFSET {page * rows} ROWS FETCH NEXT {rows} ROWS ONLY";
        }
    }
}
