using SanatoriumCore.Secure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SanService.Infrastructure
{
    public class ApiKeyAuthAttribute : AuthorizationFilterAttribute
    {
        private readonly string _apiKeyQueryParameter;

        public ApiKeyAuthAttribute(string apiKeyQueryParameter)
        {
            _apiKeyQueryParameter = apiKeyQueryParameter;
        }        

        public int Access { get; set; }

        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new List<string> { "В доступе отказано" });
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                if (AccessLevel.CheckAccess(UserRole.Public, Access))
                {
                    return;
                }
                if (!AuthorizeRequest(actionContext.ControllerContext.Request))
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
        }

        private bool AuthorizeRequest(HttpRequestMessage request)
        {
            bool authorized = false;
            try
            {
                if (request.Headers.Contains(_apiKeyQueryParameter))
                {
                    var token = request.Headers.GetValues(_apiKeyQueryParameter).First();
                    var user = AuthorizedUserRepository.GetUser(token);

                    if (user != null)
                    {
                        if (AccessLevel.CheckAccess((UserRole)user.RoleID, Access))
                        {
                            request.Properties.Add("user", user);
                            authorized = true;
                        }                       
                    }
                }
            }
            catch (Exception) { }

            return authorized;
        }
    }
}
