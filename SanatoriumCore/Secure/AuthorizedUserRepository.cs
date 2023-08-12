using SanatoriumCore.Db;
using SanatoriumEntities.Models.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SanatoriumCore.Secure
{
    public class AuthorizedUserRepository
    {
        private static Dictionary<string, LocalUser> users = DatabaseRepository.GetLoggedInUsers().ToDictionary(p => p.SessionID);
             

        public static LocalUser GetUser(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }

            if (users.ContainsKey(sessionId))
            {
                return users[sessionId];
            }

            return null;
        }

        public static void AddUser(string sessionId, LocalUser user)
        {
            if (!users.ContainsKey(sessionId))
            {
                users.Add(sessionId, user);
            }
        }

        public static void RemoveUser(string sessionId)
        {
            if (users.ContainsKey(sessionId))
            {
                users.Remove(sessionId);
            }
        }

    }
}
