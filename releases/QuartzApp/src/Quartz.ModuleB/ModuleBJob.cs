using NLog;
using Quartz.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.ModuleB
{ 
	[QuartzAppJob(15)]
	public class ModuleBJob : IJob
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private static DateTime start = DateTime.Now;

		public ModuleBJob()
		{
			logger.Trace("Creating Job B");
		}

		~ModuleBJob()
		{
			logger.Trace("Destroying Job B");
		}

		public void Execute(IJobExecutionContext context)
		{
			logger.Trace(@"Executing Job B - {0:mm\:ss\.fff}", DateTime.Now - start);
		}
	}
}
