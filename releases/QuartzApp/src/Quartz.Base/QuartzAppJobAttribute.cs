using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Base
{
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class QuartzAppJobAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly int seconds;

		// This is a positional argument
		public QuartzAppJobAttribute(int seconds)
		{
			this.seconds = seconds;
		}

		public int Seconds
		{
			get { return this.seconds; }
		}
	}
}
