using NLog;
using Quartz.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.ModuleB
{ 
	[QuartzAppJob(7)]
	public class ModuleCJob : IJob
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private static DateTime start = DateTime.Now;

		public ModuleCJob()
		{
			logger.Trace("Creating Job C");
		}

		~ModuleCJob()
		{
			logger.Trace("Destroying Job C");
		}

		public void Execute(IJobExecutionContext context)
		{
			logger.Trace(@"Executing Job C - {0:mm\:ss\.fff}", DateTime.Now - start);
		}
	}
}
