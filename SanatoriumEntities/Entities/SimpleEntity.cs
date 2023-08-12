using SanatoriumEntities.Models;

namespace SanatoriumEntities.Entities
{
    public class SimpleEntity<ModelType> : AbstractSanatoriumSimpleEntity<ModelType>
        where ModelType : BaseModel, new()
    {
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
