using System;
using System.Linq;

namespace SanatoriumCore.Secure
{
    [Flags]
    public enum UserRole : int
    {
        Admin = 1,
        Fund = 2,
        Insurance = 4,
        Policlinic = 8,
        Hospital = 16,
        Public = 32,
        User = 64,
    }

    public static class AccessLevel
    {
        public const int Public = (int)(UserRole.Public | UserRole.User | UserRole.Admin | UserRole.Fund | UserRole.Insurance | UserRole.Policlinic | UserRole.Hospital);
        public const int Anon = (int)(UserRole.Public);
        public const int User = (int)(UserRole.User | UserRole.Admin | UserRole.Fund | UserRole.Insurance | UserRole.Policlinic | UserRole.Hospital);
        public const int Admin = (int)(UserRole.Admin);
        public const int Fund = (int)(UserRole.Fund | UserRole.Admin);
        public const int Insurance = (int)(UserRole.Insurance | UserRole.Admin);
        public const int Policlinic = (int)(UserRole.Policlinic | UserRole.Admin);

        public static bool CheckAccess(UserRole @public, object access)
        {
            throw new NotImplementedException();
        }

        public const int Hospital = (int)(UserRole.Hospital | UserRole.Admin);

        public static bool CheckAccess(UserRole role, int access)
        {
            return ((int)role & access) != 0;
        }

        public static bool CheckAccess(UserRole role, params int[] access)
        {
            return access.Any(p => ((int)role & p) != 0);
        }
    }
}
