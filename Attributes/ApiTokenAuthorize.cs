using PharmYdm.BusinessLogic;
using PharmYdm.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace PharmYdm.Attributes
{
    public class ApiTokenAuthorize : AuthorizeAttribute
    {

        protected override bool IsAuthorized(HttpActionContext ctx)
        {
            var authHeader = ctx.Request.Headers.Authorization;
            var scheme = authHeader.Scheme;

            return scheme == "Token";
        }
    }
}