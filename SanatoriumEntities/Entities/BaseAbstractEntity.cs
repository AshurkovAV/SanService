using System;
using SanatoriumEntities.Models;
using SanatoriumEntities.Helpers;
using SanatoriumEntities.Helpers.SQL;
using SanatoriumEntities.Exceptions;
using SanatoriumEntities.ServicesClasses;

namespace SanatoriumEntities.Entities
{
    public abstract class BaseAbstractEntity<ModelType>
    where ModelType : BaseModel
    {
        protected int _selectableRowsCount = 99999;
        protected int _selectablePage = 0;
        public int selectablePage { get => _selectablePage; set => _selectablePage = value; }
        public int selectableRowsCount { get => _selectableRowsCount; set => _selectableRowsCount = value; }

        public int selectableStatus { get; set; } = 1;

        protected SqlSelectHelper<ModelType> sqlSelectHelper;
        protected SqlUpdateHelper<ModelType> sqlUpdateHelper;
        protected SqlInsertHelper<ModelType> sqlInsertHelper;
        protected SqlDeleteHelper<ModelType> sqlDeleteHelper;

        public BaseAbstractEntity()
        {
            this.sqlSelectHelper = new SqlSelectHelper<ModelType>(
                this.getActualTableName(),
                this.getDefaultUsername()
            );

            this.sqlUpdateHelper = new SqlUpdateHelper<ModelType>(
                this.getActualTableName(),
                this.getDefaultUsername()
            );

            this.sqlInsertHelper = new SqlInsertHelper<ModelType>(
                this.getActualTableName(),
                this.getDefaultUsername()
            );
            
            this.sqlDeleteHelper = new SqlDeleteHelper<ModelType>(
                this.getActualTableName(),
                this.getDefaultUsername()
            );
        }

        public int setRowsPerPageCount(int rowsPerPage)
        {
            _selectableRowsCount = rowsPerPage;

            return _selectableRowsCount;
        }

        public int getRowsPerPageCount()
        {
            return _selectableRowsCount;
        }


        public int checkRowsCount(string filter = "")
        {
            try
            {
                sqlSelectHelper.selectableStatus = selectableStatus;
                
                var result = SanAdmDbConn.sqlScalarQuery(sqlSelectHelper.generateQuery(true, filter));

                int count = (int) result;

                if (count > 0) {
                    return count;
                }

                return 0;
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        public int getPagesCount(string filter = "")
        {
            try
            {
                sqlSelectHelper.selectableStatus = selectableStatus;
                
                var result = SanAdmDbConn.sqlScalarQuery(sqlSelectHelper.generateQuery(true, filter));

                int count = (int) result;

                if (count > 0) {
                    int pagesCount = (count + getRowsPerPageCount() - 1) / getRowsPerPageCount();

                    return pagesCount;
                }

                return 0;
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }

        protected string getActualTableName()
        {
            return SanAdmDbConn.DB_NAME + ".dbo." + this.getDefaultTableName();
        }

        protected abstract string getDefaultTableName();

        public abstract ModelType getModel();

        protected string getDefaultUsername()
        {
            return "system";
        }

        protected ReturnType runSingleItemAction<ReturnType>(int id, Func<int, ReturnType> actionToDo)
        {
            try
            {
                return actionToDo(id);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }
        protected ReturnType runSingleModelAction<ReturnType>(ReturnType modelObj, Func<ReturnType, ReturnType> actionToDo)
        {
            try
            {
                return actionToDo(modelObj);
            }
            catch (System.Exception e)
            {
                Log.Get().put(Log.ERR, e.Message);

                ExecutionException reThrow = new ExecutionException(e.Message);
                throw reThrow;
            }
        }
    }
}
