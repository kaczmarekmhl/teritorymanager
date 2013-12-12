using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Filters
{
    public class LogAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(System.Web.Mvc.ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        public override void OnResultExecuting(System.Web.Mvc.ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }
}