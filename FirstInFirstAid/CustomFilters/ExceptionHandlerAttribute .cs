using log4net;
using System;
using System.Reflection;
using System.Web.Mvc;

namespace FirstInFirstAid.CustomFilters
{
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                logger.Error("Exception Occured", filterContext.Exception);
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Error/Index.cshtml",
                };
                filterContext.ExceptionHandled = true;
            }
        }
    }
}