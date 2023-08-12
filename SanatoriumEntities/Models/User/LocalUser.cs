using System;
using System.Collections.Generic;
using System.Text;

namespace SanatoriumEntities.Models.User
{
    public class LocalUser : BaseModel
    {
            public int?   UserId             { get; set; }
            public string Login              { get; set; }
            public string Pass               { get; set; }
            public string Salt               { get; set; }
            public bool   Active             { get; set; }
            public string LastName           { get; set; }
            public string FirstName          { get; set; }
            public string Patronymic         { get; set; }
            public string Position           { get; set; }
            public string Phone              { get; set; }
            public string ConfNumber         { get; set; }
            public string SessionID          { get; set; }
            public int?   RoleID             { get; set; }
            public string CurrentFilter      { get; set; }
            public string CurrentFilterEvent { get; set; }
            public int?   EmployeeId         { get; set; }
            
            public override string getDatabaseEntityName()
            {
                return "RslocalUser";
            }
        
    }
}
