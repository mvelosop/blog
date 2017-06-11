using NLog;
using Quartz.Impl;
using Quartz.ModuleA;
using Quartz.ModuleB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Quartz.Base;
using System.Diagnostics;
using System.IO;

namespace Quartz.App
{
	class Program
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			bool jobsScheduled = false;

			try
			{
				logger.Trace("----------------------------------------");

				logger.Trace("Staring...");

				IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

				scheduler.Start();

				logger.Trace("Loading QuartzApp modules...");

				var quartzModules = Directory.EnumerateFiles(
					AppDomain.CurrentDomain.BaseDirectory,
					"Quartz.Module*.dll",
					SearchOption.TopDirectoryOnly);

				foreach (var quartzModule in quartzModules)
				{
					try
					{
						logger.Trace("Loading Job assembly: {0}", quartzModule);
						Assembly quartzAssembly = Assembly.LoadFile(quartzModule);

						logger.Trace("Registering jobs...");

						var assemblyJobs = quartzAssembly
							.ExportedTypes
							.Where(et => et.GetCustomAttribute<QuartzAppJobAttribute>() != null);

						foreach (var assemblyJob in assemblyJobs)
						{
							IJobDetail job = JobBuilder.Create(assemblyJob)
								.Build();

							int seconds = assemblyJob.GetCustomAttribute<QuartzAppJobAttribute>().Seconds;

							ITrigger trigger = TriggerBuilder.Create()
								.StartNow()
								.WithSimpleSchedule(x => x
									.WithIntervalInSeconds(seconds)
									.RepeatForever())
								.Build();

							logger.Trace("Scheduling Job {0} with {1} seconds lapse", assemblyJob.Name, seconds);
							scheduler.ScheduleJob(job, trigger);

							jobsScheduled = true;
						}

					}
					catch (Exception ex)
					{
						logger.Error(ex);
					}

				}

				if (jobsScheduled)
				{
					Thread.Sleep(TimeSpan.FromSeconds(30));
				}

				scheduler.Shutdown();

			}
			catch (Exception ex)
			{
				logger.Error(ex);

				throw;
			}
		}
	}
}
