using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SanatoriumCore.Secure;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace SanService.Infrastructure.Filters
{
    public class SimpleResourceFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context != null)
                {
                    var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
                    var claimsIndentity = context.HttpContext.User.Identity as ClaimsIdentity;
                    if (context.HttpContext.Request.Path == "/auth/Login" || context.HttpContext.Request.Path == "/Authorization/Index")
                    { return; }
                    if (!IsAuthenticated)
                    {
                        throw new Exception();
                    }
                    foreach (var d in claimsIndentity.Claims)
                    {
                        if (d.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")
                        {
                            var token = d.Value;
                            var user = AuthorizedUserRepository.GetUser(token);
                            if (user != null)
                            {
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }

                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new ObjectResult(HttpStatusCode.Unauthorized);                
            }                      
        }  
    }
}