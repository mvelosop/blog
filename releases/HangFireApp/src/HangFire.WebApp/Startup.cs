using Hangfire;
using HangFire.Base;
using HangFire.JobOne;
using Microsoft.Owin;
using NLog;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(HangFire.WebApp.Startup))]
namespace HangFire.WebApp
{
	public partial class Startup
	{
		private Logger logger = LogManager.GetCurrentClassLogger();

		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);

			GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

			app.UseHangfireDashboard();
			app.UseHangfireServer();

			ScheduleRecurringJobs();
		}

		private void ScheduleRecurringJobs()
		{
			try
			{
				logger.Trace("----------------------------------------");

				logger.Trace("Starting...");

				logger.Trace("Loading HangFire modules...");

				var hangFireModules = Directory.EnumerateFiles(
					Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"),
					"HangFire.Job*.dll",
					SearchOption.TopDirectoryOnly);

				foreach (var module in hangFireModules)
				{
					try
					{
						logger.Trace("Loading Job assembly: {0}", module);
						Assembly assembly = Assembly.LoadFile(module);

						logger.Trace("Getting jobs...");

						var assemblyJobs = assembly
							.ExportedTypes
							.Where(et => et.GetCustomAttribute<HangFireJobAttribute>() != null);

						foreach (Type job in assemblyJobs)
						{
							int minutes = job.GetCustomAttribute<HangFireJobAttribute>().Minutes;

							logger.Trace("Scheduling Job {0} with {1} minutes interval", job.Name, minutes);

							MethodInfo executeMethod = job.GetMethod("Execute");

							Expression<Action> expression = Expression.Lambda<Action>(Expression.Call(executeMethod));

							RecurringJob.AddOrUpdate(job.FullName, expression, Cron.MinuteInterval(minutes));
						}

					}
					catch (Exception ex)
					{
						logger.Error(ex);
					}

				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);

				throw;
			}
		}
	}
}
