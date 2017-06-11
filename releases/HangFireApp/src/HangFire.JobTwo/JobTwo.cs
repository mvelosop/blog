using HangFire.Base;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangFire.JobTwo
{
	[HangFireJob(2)]
	public class JobTwo
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private static DateTime start = DateTime.Now;

		public static Task Execute()
		{
			return Task.Run(() => logger.Trace(@"Executing Job Two - {0:mm\:ss\.fff}", DateTime.Now - start));
		}
	}
}
