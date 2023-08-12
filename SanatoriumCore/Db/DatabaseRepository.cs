using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanatoriumCore.Db
{
    public static class DatabaseRepository
    {
        private static SimpleEntity<LocalUser> _resources = new SimpleEntity<LocalUser>();

        internal static IEnumerable<LocalUser> GetLoggedInUsers()
        {
            try
            {
                return _resources.selectList("SessionID is not null and SessionID != '' and Active = 1", "Userid");
            }
            catch (Exception exception)
            {

            }
            return null;
        }
        
    }
}
