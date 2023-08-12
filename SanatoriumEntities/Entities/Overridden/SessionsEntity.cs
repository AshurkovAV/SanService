using SanatoriumEntities.ServicesClasses;
using SanatoriumEntities.Models.Session;
using SanatoriumEntities.Models.Customer;

namespace SanatoriumEntities.Entities.Overriden
{
    public class SessionsEntity : SimpleEntity<Session> 
    {
        public override int delete(int id, bool isHardDelete = false)
        {
            SEntities.AppointedPrograms().deleteList($"csession_id={id}", isHardDelete);
            SEntities.CustomersSchedules().deleteList($"csession_id={id}", isHardDelete);
            
            SEntities.GetEntity<CardRecord>().deleteList($"csession_id={id}", isHardDelete);
            SEntities.GetEntity<Contract>().deleteList($"csession_id={id}", isHardDelete);
            
            return base.delete(id, isHardDelete);
        }
    }
}
