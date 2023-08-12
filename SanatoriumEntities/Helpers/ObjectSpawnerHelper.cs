using System;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SanatoriumEntities.Helpers
{
    public class ObjectSpawnerHelper<ModelType>
    where ModelType : class
    {
        public static ModelType spawnModelObjectBySqlStatement(Func <ModelType> modelSpawner, string queryString)
        {
            ModelType modelObject = modelSpawner();

            using (var connection = new SqlConnection(SanAdmDbConn.getConnectionString()))
            using (SqlCommand command = new SqlCommand(queryString, connection))
            { 
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    ObjectSpawnerHelper<ModelType>.fillModel(modelObject, reader);
                }
            }
            
            return modelObject;
        }

        public static List<ModelType> spawnModelObjectBySqlStatementsListBySqlStatement(Func <ModelType> modelSpawner, string queryString)
        {
            List<ModelType> modelsList = new List<ModelType>();

            using (var connection = new SqlConnection(SanAdmDbConn.getConnectionString()))
            using (SqlCommand command = new SqlCommand(queryString, connection))
            { 
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ModelType modelObject = modelSpawner();

                        ObjectSpawnerHelper<ModelType>.fillModel(modelObject, reader);

                        modelsList.Add(modelObject);
                    }
                }
            }
            
            return modelsList;
        }

        protected static ModelType fillModel(ModelType modelObject, SqlDataReader reader)
        {
            PropertyInfo[] properties = modelObject.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            if (!reader.HasRows) {
                return null;
            }

            foreach (var p in properties)
            {
                if (!ObjectSpawnerHelper<ModelType>.ColumnExists(reader, p.Name)) {
                    continue;
                }

                if (reader.IsDBNull(reader.GetOrdinal(p.Name))) {
                    continue;
                }
                
                switch (p.PropertyType.ToString())
                {
                    case "System.String":
                    case "System.Nullable`1[System.String]":
                    
                        p.SetValue(
                            modelObject,
                            reader.GetString(reader.GetOrdinal(p.Name))
                        );

                        break;

                    case "System.Int32":
                    case "System.Nullable`1[System.Int32]":
                        
                        p.SetValue(
                            modelObject,
                            reader.GetInt32(reader.GetOrdinal(p.Name))
                        );

                        break;

                    case "System.Boolean":
                    case "System.Nullable`1[System.Boolean]":
                                                
                        p.SetValue(
                            modelObject,
                            reader.GetBoolean(reader.GetOrdinal(p.Name))
                        );

                        break;
                    
                    case "System.DateTime":
                    case "System.Nullable`1[System.DateTime]":
                        
                        p.SetValue(
                            modelObject,
                            reader.GetDateTime(reader.GetOrdinal(p.Name))
                        );

                        break;
                }
            }

            return modelObject;
        }

        public static bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}