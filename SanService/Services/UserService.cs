using SanatoriumEntities.Entities;
using System;
using System.Linq;
using SanatoriumEntities.Models.User;
using SanatoriumCore.Helpers;
using SanatoriumCore.Infrastructure;
using SanatoriumCore.Secure;
using SanatoriumEntities.Models.Services;

namespace SanService.Services
{
    public class UserService
    {
        private SimpleEntity<LocalUser>           _userResources = new SimpleEntity<LocalUser>();
        private SimpleEntity<EmployeeBinding>     _simpleEntityBin = new SimpleEntity<EmployeeBinding>();
        private SimpleEntity<GeneralEmployeeItem> _simpleEntityEmpGen = new SimpleEntity<GeneralEmployeeItem>();

        public string ErrorMessage { get; private set; }

        public TransactionResult<LocalUser> LoginUser(string login, string password)
        {
            var result = new TransactionResult<LocalUser>();
            try
            {

                var user = _userResources.selectList($"Login = '{login}'", "UserID").FirstOrDefault();
                if (user == null)
                {
                    ErrorMessage = "Пользователь не найден";
                    throw new Exception("Пользователь не найден");
                }
                if ((bool)!user.Active)
                {
                    ErrorMessage = "Пользователь заблокирован";
                    throw new Exception("Пользователь заблокирован");
                }
                if (CryptoHelpers.ConfirmPassword(password, Convert.FromBase64String(user.Pass),
                            Convert.FromBase64String(user.Salt)))
                {
                    var sid = Convert.ToBase64String(CryptoHelpers.CreateRandom(32));
                    user.SessionID = sid;
                    var resultUser = _userResources.update(user);
                   
                    AuthorizedUserRepository.AddUser(sid, user);
                    
                    result.Data = user;
                    return result;
                }
                else
                {
                    result.AddError("Логин или пароль введены неверно");
                }
            }           
            catch (Exception exception)
            {
                result.AddError(exception.Message);
                ErrorMessage = "Ошибка при попытке войти в систему";
            }

            return result;
        }


        public TransactionResult<GeneralEmployeeItem> GetEmpGenToid(int empid)
        {
            var result = new TransactionResult<GeneralEmployeeItem>();
            try
            {

                var user = _simpleEntityBin.selectList($"employee_id = {empid}").FirstOrDefault();
                var empGen = _simpleEntityEmpGen.selectList($"id = {user.employee_general_id}").FirstOrDefault();

                if (empGen != null && empGen.id != 0)
                {
                    result.Data = empGen;
                }
                else
                {
                    throw new Exception("Специалист не найдет");
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception.Message);
            }

            return result;
        }

    }
}
