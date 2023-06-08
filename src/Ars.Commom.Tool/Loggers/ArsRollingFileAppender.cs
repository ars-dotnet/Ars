using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Loggers
{
    internal class ArsRollingFileAppender : RollingFileAppender
    {
        /// <summary>
        /// 这一步不生成日志文件
        /// </summary>
        public override void ActivateOptions()
        {
            return;
        }

        /// <summary>
        /// 这一步生成日志文件
        /// </summary>
        public void ActivateOptionsTrue()
        {
            base.ActivateOptions();
        }
    }
}
