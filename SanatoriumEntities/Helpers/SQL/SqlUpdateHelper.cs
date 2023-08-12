using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Models;
using SanatoriumEntities.Models.Services;
using SanatoriumEntities.Models.User;

namespace SanatoriumEntities.Helpers.SQL
{
    public class SqlUpdateHelper<ModelType> : SqlAbstractHelper<ModelType>
    where ModelType : BaseModel
    {
        protected readonly List<string> doNotUpdateFields = new List<string>() { ID_FIELD, USERID_FIELD, STATUS, CREATED_AT, CREATED_BY };

        public SqlUpdateHelper(string tableName, string username) : base(tableName, username) { }

        public string generateQuery(ModelType modelObject)
        {
            if (!(modelObject is LocalUser))
            {
                if (0 == (modelObject.id ?? 0))
                {
                    ArgumentException argumentException = new ArgumentException(
                        String.Format(
                            "Method: [{1}], message: [empty field: [id]]",
                            this.GetType(),
                            System.Reflection.MethodBase.GetCurrentMethod()
                        )
                    );

                    throw argumentException;
                }
            }

            string conditions = $"{ID_FIELD}={modelObject.id} AND {STATUS}=1";//Allowed only for active rows
            string values = "";

            

                PropertyInfo[] properties = modelObject.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            if ((modelObject is LocalUser))
            {
                 conditions = $"Userid={properties[0].GetValue(modelObject)} ";
            }

            foreach (var p in properties)
            {
                object[] customAttributes = p.GetCustomAttributes(typeof(ServiceAttribute), false);

                bool skipProperty = false;

                foreach (ServiceAttribute customAttribute in customAttributes)
                {
                    if (GlobalConst.SKIP_IN_SQL == customAttribute.Name)
                    {
                        skipProperty = true;
                    }
                }

                if (skipProperty)
                {
                    continue;
                }

                object value = p.GetValue(modelObject);

                if (doNotUpdateFields.Contains(p.Name))
                {
                    continue;
                }
                else if (UPDATED_AT == p.Name)
                {
                    values = $"{values},{UPDATED_AT}=getdate()";

                    continue;
                }
                else if (IP4 == p.Name && null == value)
                {

                    continue;
                }
                else if (null == value)
                {
                    values = $"{values},{p.Name}=NULL";

                    continue;
                }

                switch (p.PropertyType.ToString())
                {
                    case "System.String":
                    case "System.Nullable`1[System.String]":
                        string strval = (string)value;
                        values = $"{values},{p.Name}=N'{strval}'";

                        break;

                    case "System.Int32":
                    case "System.Nullable`1[System.Int32]":
                        int intval = (int)value;
                        values = $"{values},{p.Name}={intval}";

                        break;

                    case "System.Boolean":
                    case "System.Nullable`1[System.Boolean]":
                        int bolval = (bool)value ? 1 : 0;
                        values = $"{values},{p.Name}={bolval}";

                        break;

                    case "System.DateTime":
                    case "System.Nullable`1[System.DateTime]":
                        string sqlFormattedDate;
                        DateTime dt = (DateTime)value;
                        if (dt.ToBinary() > 0)
                        {
                            sqlFormattedDate = $"'{dt.Year}{dt.Month.ToString("00")}{dt.Day.ToString("00")}'";
                            if (dt.Hour != 0 || dt.Minute != 0 || dt.Second != 0)
                            {
                                if (sqlFormattedDate.EndsWith("'"))
                                {
                                    sqlFormattedDate = sqlFormattedDate.TrimEnd('\'');
                                }
                                sqlFormattedDate = $"{sqlFormattedDate} {dt.Hour.ToString("00")}:{dt.Minute.ToString("00")}:{dt.Second.ToString("00")}'";
                            }
                        }
                        else
                        {
                            sqlFormattedDate = "NULL";
                        }


                        values = $"{values},{p.Name}={sqlFormattedDate}";

                        break;
                }

                if (values.StartsWith(","))
                {
                    values = values.TrimStart(',');
                }
                values = values.Trim();
            }

            string updateStatement = $"UPDATE {this.table} SET {values} WHERE {conditions}";

            return updateStatement;
        }

        public string generateQuery(List<ModelType> modelsObjectsList)
        {
            if (modelsObjectsList.Count == 0)
            {
                throw new ArgumentNullException("Empty objects models list");
            }

            string updateStatement = "";

            foreach (ModelType modelObject in modelsObjectsList)
            {
                string conditions = $"{ID_FIELD}={modelObject.id} AND {STATUS}=1";//Allowed only for active rows
                string values = "";

                PropertyInfo[] properties = modelObject.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
                );

                foreach (var p in properties)
                {
                    object[] customAttributes = p.GetCustomAttributes(typeof(ServiceAttribute), false);

                    bool skipProperty = false;

                    foreach (ServiceAttribute customAttribute in customAttributes)
                    {
                        if (GlobalConst.SKIP_IN_SQL == customAttribute.Name)
                        {
                            skipProperty = true;
                        }
                    }

                    if (skipProperty)
                    {
                        continue;
                    }
                    object value = p.GetValue(modelObject);

                    if (doNotUpdateFields.Contains(p.Name))
                    {
                        continue;
                    }
                    else if (UPDATED_AT == p.Name)
                    {
                        values = $"{values},{UPDATED_AT}=getdate()";

                        continue;
                    }
                    else if (IP4 == p.Name && null == value)
                    {

                        continue;
                    }
                    else if (null == value)
                    {
                        values = $"{values},{p.Name}=NULL";

                        continue;
                    }

                    switch (p.PropertyType.ToString())
                    {
                        case "System.String":
                        case "System.Nullable`1[System.String]":
                            string strval = (string)value;
                            values = $"{values},{p.Name}=N'{strval}'";

                            break;

                        case "System.Int32":
                        case "System.Nullable`1[System.Int32]":
                            int intval = (int)value;
                            values = $"{values},{p.Name}={intval}";

                            break;

                        case "System.Boolean":
                        case "System.Nullable`1[System.Boolean]":
                            int bolval = (bool)value ? 1 : 0;
                            values = $"{values},{p.Name}={bolval}";

                            break;

                        case "System.DateTime":
                        case "System.Nullable`1[System.DateTime]":
                            string sqlFormattedDate;
                            DateTime dt = (DateTime)value;
                            if (dt.ToBinary() > 0)
                            {
                                sqlFormattedDate = $"'{dt.Year}{dt.Month.ToString("00")}{dt.Day.ToString("00")}'";
                                if (dt.Hour != 0 || dt.Minute != 0 || dt.Second != 0)
                                {
                                    if (sqlFormattedDate.EndsWith("'"))
                                    {
                                        sqlFormattedDate = sqlFormattedDate.TrimEnd('\'');
                                    }
                                    sqlFormattedDate = $"{sqlFormattedDate} {dt.Hour.ToString("00")}:{dt.Minute.ToString("00")}:{dt.Second.ToString("00")}'";
                                }
                            }
                            else
                            {
                                sqlFormattedDate = "NULL";
                            }


                            values = $"{values},{p.Name}={sqlFormattedDate}";

                            break;
                    }

                    if (values.StartsWith(","))
                    {
                        values = values.TrimStart(',');
                    }
                    values = values.Trim();
                }

                updateStatement += $" UPDATE {this.table} SET {values} WHERE {conditions}";
            }

            return updateStatement;
        }
    }
}