using System;
using System.Collections.Generic;
using SanatoriumEntities.Models;
using SanatoriumEntities.Interfaces;

namespace SanatoriumEntities.Entities
{
    public abstract class AbsrtactSanatoriumAggregate<ModelType, CompleteModel> : BaseAbstractEntity<ModelType>,
        ISanatoriumAggregateEntity<ModelType>
    where ModelType: BaseModel, new()
    where CompleteModel : BaseModel
    {
        public virtual ModelType select(int id)
        {
            NotImplementedException immplementationException = new NotImplementedException(String.Format("Method: [{0}] not implemented", System.Reflection.MethodBase.GetCurrentMethod()));

            throw immplementationException;
        }

        public virtual List<ModelType> selectList(List<int> ids)
        {
            NotImplementedException immplementationException = new NotImplementedException(String.Format("Method: [{0}] not implemented", System.Reflection.MethodBase.GetCurrentMethod()));

            throw immplementationException;
        }

        public virtual List<ModelType> selectList(string filter = "", string orderByFields = "id")
        {
            NotImplementedException immplementationException = new NotImplementedException(String.Format("Method: [{0}] not implemented", System.Reflection.MethodBase.GetCurrentMethod()));

            throw immplementationException;
        }

        public virtual List<ModelType> selectList(string filter, List<string> fields, List<string> orderByFields)
        {
            NotImplementedException immplementationException = new NotImplementedException(String.Format("Method: [{0}] not implemented", System.Reflection.MethodBase.GetCurrentMethod()));

            throw immplementationException;
        }

        public virtual List<ModelType> selectList(List<int> ids, string orderByFields = "id")
        {
            NotImplementedException immplementationException = new NotImplementedException(String.Format("Method: [{0}] not implemented", System.Reflection.MethodBase.GetCurrentMethod()));

            throw immplementationException;
        }

        protected override string getDefaultTableName()
        {
            ModelType obj = getModel();
            
            return obj.getDatabaseEntityName();
        }

        public override ModelType getModel()
        {
            return new ModelType();
        }
    }
}
