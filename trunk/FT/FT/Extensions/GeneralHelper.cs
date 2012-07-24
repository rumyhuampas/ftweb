using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT.Extensions
{
    public static class GeneralHelper
    {
        public static Boolean DEBUG()
        {
            var value = false;
            #if(DEBUG)
            value = true;
            #endif
            return value;
            //return HttpContext.Current.IsDebuggingEnabled;
        }
    }
}