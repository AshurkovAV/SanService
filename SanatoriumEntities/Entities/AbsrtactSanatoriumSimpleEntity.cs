using System;
using System.Linq;
using System.Collections.Generic;
using SanatoriumEntities.Interfaces;
using SanatoriumEntities.Models;
using SanatoriumEntities.Helpers;
using SanatoriumEntities.Exceptions;
using SanatoriumEntities.ServicesClasses;

namespace SanatoriumEntities.Entities
{
    public abstract class AbstractSanatoriumSimpleEntity<ModelType> :
        BaseAbstractEntity<ModelType>,
        ISanatoriumSimpleEntity<ModelType>
    where ModelType     : BaseModel
    {
        public ModelType select(int id)
        {
            return runSingleItemAction(id, this.selectProcessing);
        }

        public List<ModelType> selectList(string filter, string orderByFields = "id")
        {
            try
            {
                sqlSelectHelper.selectableRowsCount = selectableRowsCount;
                sqlSelectHelper.selectablePage      = selectablePage;
                sqlSelectHelper.selectableStatus    = selectableStatus;

                return ObjectSpawnerHelper<ModelType>.spawnModelObjectBySqlStatementsListBySqlStatement(
                    this.getModel,
                    sqlSelectHelper.generateQuery(filter, orderByFields)
                );
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public List<ModelType> selectList(string filter, List<string> fields, List<string> orderByFields)
        {
            try
            {
                sqlSelectHelper.selectableRowsCount = selectableRowsCount;
                sqlSelectHelper.selectablePage      = selectablePage;
                sqlSelectHelper.selectableStatus    = selectableStatus;

                return ObjectSpawnerHelper<ModelType>.spawnModelObjectBySqlStatementsListBySqlStatement(
                    this.getModel,
                    sqlSelectHelper.generateQuery(filter, fields, orderByFields)
                );
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public List<ModelType> selectList(List<int> ids)
        {
            try
            {
                sqlSelectHelper.selectableRowsCount = selectableRowsCount;
                sqlSelectHelper.selectablePage      = selectablePage;
                sqlSelectHelper.selectableStatus    = selectableStatus;

                List<ModelType> result =  ObjectSpawnerHelper<ModelType>.spawnModelObjectBySqlStatementsListBySqlStatement(
                    this.getModel,
                    sqlSelectHelper.generateQuery(ids)
                );

                if (ids.Count != result.Count)
                {
                    throw new IvalidExpectedDataException($"Input: [{(String.Join(",", ids))}].\nExpected: [{ids.Count.ToString("000")}].\nActual: [{result.Count.ToString("000")}]");
                }

                return result;
                
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public int insert(ModelType modelObj)
        {
            try
            {
                return this.insertProcessing(modelObj);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public int insertList(List<ModelType> modelsObjectsList)
        {
            try
            {
                if (modelsObjectsList.Count == 0) {
                    return 0;
                }
                
                var result = SanAdmDbConn.sqlQuery(
                    sqlInsertHelper.generateQuery(modelsObjectsList)
                );

                return (int) (decimal) result;
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public int updateList(List<ModelType> modelsObjectsList)
        {
            try
            {
                if (modelsObjectsList.Count == 0) {
                    return 0;
                }
                
                var result = SanAdmDbConn.sqlQuery(
                    sqlUpdateHelper.generateQuery(modelsObjectsList)
                );

                return (int) (decimal) result;
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }
        public int deleteList(List<int> ids, bool isHardDelete = false)
        {
            try
            {
                var result = SanAdmDbConn.sqlQueryInTransaction(
                    sqlDeleteHelper.generateQuery(ids, false, isHardDelete),
                    ids.Count
                );

                return result;
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public virtual int deleteList(string filter, bool isHardDelete = false)
        {
            try
            {
                List<ModelType> items = selectList(filter, new List<string>() {"id"}, new List<string>() {"id"});
                List<int> ids = new List<int>();

                foreach (ModelType item in items)
                {
                    ids.Add(item.id ?? 0);
                }
                
                return deleteList(ids, isHardDelete);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public int update(ModelType modelObj)
        {
            try
            {
                return this.updateProcessing(modelObj);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public ModelType insertAndSelect(ModelType modelObj)
        {
            try
            {
                int id = this.insertProcessing(modelObj);

                return this.select(id);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public ModelType updateAndSelect(ModelType modelObj)
        {
            try
            {
                this.updateProcessing(modelObj);

                return this.select(modelObj.id ?? 0);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public virtual int delete(int id, bool isHardDelete = false)
        {
            try {
                var result = SanAdmDbConn.sqlQuery(sqlDeleteHelper.generateQuery(id, false, isHardDelete));

                if (result > 0) {
                    return id;
                }

                return 0;
            } catch (System.Exception e) {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public int restore(int id)
        {
            try {
                var result = SanAdmDbConn.sqlQuery(sqlDeleteHelper.generateQuery(id, true));

                if (result > 0) {
                    return id;
                }

                return 0;
            } catch (System.Exception e) {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }
        
        public ModelType deleteAndSelect(int id)
        {
            return runSingleItemAction(id, this.deleteAndSelectProcessing);
        }

        public ModelType restoreAndSelect(int id)
        {
            return runSingleItemAction(id, this.restoreAndSelectProcessing);
        }

        protected ModelType selectProcessing(int id)
        {
            sqlSelectHelper.selectableRowsCount = selectableRowsCount;
            sqlSelectHelper.selectablePage      = selectablePage;
            sqlSelectHelper.selectableStatus    = selectableStatus;

            ModelType modelObject = ObjectSpawnerHelper<ModelType>.spawnModelObjectBySqlStatement(
                this.getModel,
                sqlSelectHelper.generateQuery(id)
            );

            return modelObject;
        }

        protected int insertProcessing(ModelType model)
        {
            var result = SanAdmDbConn.sqlScalarQuery(
                sqlInsertHelper.generateQuery(model, true)
            );

            return (int) (decimal) result;
        }

        protected int updateProcessing(ModelType modelObj)
        {
            var result = SanAdmDbConn.sqlQuery(
                sqlUpdateHelper.generateQuery(modelObj)
            );

            if (result > 0) {
                return 1;
            }

            return 0;
        }

        protected ModelType deleteAndSelectProcessing(int id)
        {
            sqlSelectHelper.selectableRowsCount = selectableRowsCount;
            sqlSelectHelper.selectablePage      = selectablePage;
            sqlSelectHelper.selectableStatus    = 0;

            ModelType result = ObjectSpawnerHelper<ModelType>.spawnModelObjectBySqlStatement(
                this.getModel,
                sqlDeleteHelper.generateQuery(id) + " " + sqlSelectHelper.generateQuery(id)
            );

            return result;
        }

        protected ModelType restoreAndSelectProcessing(int id)
        {
            sqlSelectHelper.selectableRowsCount = selectableRowsCount;
            sqlSelectHelper.selectablePage      = selectablePage;
            sqlSelectHelper.selectableStatus    = 1;

            ModelType result = ObjectSpawnerHelper<ModelType>.spawnModelObjectBySqlStatement(
                this.getModel,
                sqlDeleteHelper.generateQuery(id, true) + " " + sqlSelectHelper.generateQuery(id)
            );
 
            return result;
        }
    }
}
