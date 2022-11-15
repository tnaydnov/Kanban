using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ServiceStack.Text;

namespace IntroSE.Kanban.Backend.Utility
{
    public class Logger
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog GetLogger()
        {
            return logger;
        }

    }
}
