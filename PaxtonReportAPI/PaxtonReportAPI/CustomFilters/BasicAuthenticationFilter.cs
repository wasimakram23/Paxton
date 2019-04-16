using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Text;
using System.Threading;
using System.Security.Principal;
namespace PaxtonReportAPI.CustomFilters
{
    public class BasicAuthenticationFilter: AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string authToken = actionContext.Request.Headers.Authorization.Parameter;
                string decodeauthToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                string[] userpassword = decodeauthToken.Split(':');
                string user = userpassword[0];
                string password = userpassword[1];
                if (new UserChecker().Login(user, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(user), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
    }

    internal class UserChecker
    {
        public UserChecker()
        {
         
        }
        public bool Login(string user, string password)
        {
            if (user.Equals("paxton") && password.Equals("p@xton@123"))
                return true;
            else
                return false;
        }
    }
}