using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Models;

namespace SanatoriumEntities.Helpers.SQL
{
    public class SqlInsertHelper<ModelType> : SqlAbstractHelper<ModelType>
    where ModelType : BaseModel
    {
        protected readonly List<string> doNotInsertFields = new List<string>() {ID_FIELD, STATUS, CREATED_AT, UPDATED_AT, UPDATED_BY};

        public SqlInsertHelper(string tableName, string username) : base(tableName, username) { }

        public string generateQuery(ModelType modelObject, bool isNeedSelectId = false)
        {
            string fields = "";
            string values = "";

            PropertyInfo[] properties = modelObject.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            foreach (var p in properties)
            {
                if (doNotInsertFields.Contains(p.Name)) {
                    continue;
                }

                object value = p.GetValue(modelObject);

                if (IP4 == p.Name && null == value) {
                    value = 2130706433;
                } else if (null == value) {
                    continue;
                } else if ("System.DateTime" == p.PropertyType.ToString() && 0 == ((DateTime)value).ToBinary()) {
                    continue;
                }

                fields = $"{fields ?? ""},{p.Name}";

                switch (p.PropertyType.ToString())
                {
                    case "System.String":
                        string strval = (string)value;
                        values = $"{values},N'{strval}'";

                        break;

                    case "System.Int32":
                    case "System.Nullable`1[System.Int32]":
                        int intval = (int)value;
                        values = $"{values},{intval}";

                        break;

                    case "System.Boolean":
                    case "System.Nullable`1[System.Boolean]":
                        int bolval = (bool)value ? 1 : 0;
                        values = $"{values},{bolval}";

                        break;
                    
                    case "System.DateTime":
                    case "System.Nullable`1[System.DateTime]":
                        DateTime dt = (DateTime)value;
                        if (0 == dt.ToBinary()) {
                            continue;
                        }

                        string sqlFormattedDate = $"{dt.Year}{dt.Month.ToString("00")}{dt.Day.ToString("00")}";
                        if (dt.Hour != 0 || dt.Minute != 0 || dt.Second != 0) {
                            sqlFormattedDate = $"{sqlFormattedDate} {dt.Hour.ToString("00")}:{dt.Minute.ToString("00")}:{dt.Second.ToString("00")}";
                        }
                        values = $"{values},'{sqlFormattedDate}'";

                        break;
                }
            }

            if (fields.StartsWith(",")) {
                fields = fields.TrimStart(',');
            }
            fields = fields.Trim();
            
            if (values.StartsWith(",")) {
                values = values.TrimStart(',');
            }
            values = values.Trim();

            string insertStatement = $"INSERT INTO {this.table} ({fields}) VALUES ({values})";
            if (isNeedSelectId) {
                insertStatement = insertStatement + " SELECT SCOPE_IDENTITY() as id";
            }

            return insertStatement;
        }

        public string generateQuery(List<ModelType> modelsObjectsList)
        {
            if (modelsObjectsList.Count == 0)
            {
                throw new ArgumentNullException("Empty objects models list");
            }

            string fields = "";
            string values = "";

            ModelType firstModel = modelsObjectsList.FirstOrDefault();

            PropertyInfo[] properties = firstModel.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            int chunksCount = 0;
            string insertStatement = "";

            foreach (ModelType model in modelsObjectsList)
            {
                string valuesChunk = "";

                foreach (var p in properties)
                {
                    if (doNotInsertFields.Contains(p.Name)) {
                        continue;
                    }

                    object value = p.GetValue(model);

                    if (IP4 == p.Name && null == value) {
                        value = 2130706433;
                    } else if (null == value) {
                        continue;
                    } else if ("System.DateTime" == p.PropertyType.ToString() && 0 == ((DateTime)value).ToBinary()) {
                        continue;
                    }

                    if (0 == chunksCount) fields = $"{fields ?? ""},{p.Name}";

                    switch (p.PropertyType.ToString())
                    {
                        case "System.String":
                            string strval = (string)value;
                            valuesChunk = $"{valuesChunk},N'{strval}'";

                            break;

                        case "System.Int32":
                        case "System.Nullable`1[System.Int32]":
                            int intval = (int)value;
                            valuesChunk = $"{valuesChunk},{intval}";

                            break;

                        case "System.Boolean":
                        case "System.Nullable`1[System.Boolean]":
                            int bolval = (bool)value ? 1 : 0;
                            valuesChunk = $"{valuesChunk},{bolval}";

                            break;
                        
                        case "System.DateTime":
                            DateTime dt = (DateTime)value;
                            if (0 == dt.ToBinary()) {
                                continue;
                            }

                            string sqlFormattedDate = $"{dt.Year}{dt.Month.ToString("00")}{dt.Day.ToString("00")}";
                            if (dt.Hour != 0 || dt.Minute != 0 || dt.Second != 0) {
                                sqlFormattedDate = $"{sqlFormattedDate} {dt.Hour.ToString("00")}:{dt.Minute.ToString("00")}:{dt.Second.ToString("00")}";
                            }
                            
                            valuesChunk = $"{valuesChunk},'{sqlFormattedDate}'";

                            break;
                    }
                }

                if (valuesChunk.StartsWith(",")) {
                    valuesChunk = valuesChunk.TrimStart(',');
                }
                valuesChunk = valuesChunk.Trim();

                values = $"{values}, ({valuesChunk})";

                if (0 == chunksCount % 999) {
                    if (values.StartsWith(",")) {
                        values = values.TrimStart(',');
                    }
                    values = values.Trim();

                    if (fields.StartsWith(",")) {
                        fields = fields.TrimStart(',');
                    }
                    fields = fields.Trim();
    
                    insertStatement = $"{insertStatement} INSERT INTO {this.table} ({fields}) VALUES {values}";
                    values = "";
                }

                chunksCount++;
            }

            if (values.Length > 0) {
                if (values.StartsWith(",")) {
                    values = values.TrimStart(',');
                }
                values = values.Trim();

                if (fields.StartsWith(",")) {
                    fields = fields.TrimStart(',');
                }
                fields = fields.Trim();
                
                insertStatement = $"{insertStatement} INSERT INTO {this.table} ({fields}) VALUES {values}";
                insertStatement = insertStatement.Trim();
            }

            return insertStatement;
        }
    }
}