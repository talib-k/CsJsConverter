using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CsJsConversion.BaseClassMembers
{
    public class MvcInfo
    {
        public virtual IHtmlString Action(string actionName)
        {
            return Action(actionName, null, null);
        }

        public virtual IHtmlString Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, (object)null);
        }

        public virtual IHtmlString Action(string actionName, string controllerName, IDictionary<string, object> routeValues)
        {
            return Action(actionName, controllerName, ToAnonymousObject(routeValues));
        }

        public virtual IHtmlString Action(string actionName, string controllerName, object routeValues)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var requestContext = new RequestContext(httpContext, RouteTable.Routes.GetRouteData(httpContext));
            var urlHelper = new UrlHelper(requestContext);
            var url = urlHelper.Action(actionName, controllerName, routeValues, HttpContext.Current.Request.Url.Scheme);
            return MvcHtmlString.Create(url);
        }

        protected static object ToAnonymousObject(IDictionary<string, object> dictionary)
        {
            var expandoObject = new ExpandoObject();

            if (dictionary != null)
            {
                var expandoDictionary = (IDictionary<string, object>)expandoObject;
                foreach (var keyValuePair in dictionary)
                {
                    expandoDictionary.Add(keyValuePair);
                }
            }
            return expandoObject;
        }

    }
}
