using NLog;
using Quartz.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.ModuleA
{
	[QuartzAppJob(5)]
	public class ModuleAJob : IJob
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private static DateTime start = DateTime.Now;

		public ModuleAJob()
		{
			logger.Trace("Creating Job A");
		}

		~ModuleAJob()
		{
			logger.Trace("Destroying Job A");
		}

		public void Execute(IJobExecutionContext context)
		{
			logger.Trace(@"Executing Job A - {0:mm\:ss\.fff}", DateTime.Now - start);
		}
	}
}
