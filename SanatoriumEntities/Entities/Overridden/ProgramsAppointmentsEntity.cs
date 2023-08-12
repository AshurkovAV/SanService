using System.Collections.Generic;
using SanatoriumEntities.ServicesClasses;
using SanatoriumEntities.Models.Session;

namespace SanatoriumEntities.Entities.Overriden
{
    public class ProgramsAppointmentsEntity : SimpleEntity<ProgramAppointment> 
    {
        public override int delete(int id, bool isHardDelete = false)
        {
            SEntities.Orders().deleteList($"program_appointment_id={id}", isHardDelete);
                       
            return base.delete(id, isHardDelete);
        }

        public override int deleteList(string filter, bool isHardDelete = false)
        {
            int res = 0;

            List<ProgramAppointment> items = SEntities.AppointedPrograms(true).selectList(filter, "id");

            foreach (ProgramAppointment item in items)
            {
                try {
                    delete(item.id ?? 0, isHardDelete);
                    res += 1;
                } catch (System.Exception e) {
                    Log.Get().put(Log.ERR, e.Message);
                }
            }

            return res;
        }
    }
}
