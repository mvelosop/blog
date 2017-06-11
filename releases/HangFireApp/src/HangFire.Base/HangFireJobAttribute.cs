using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangFire.Base
{
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class HangFireJobAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly int minutes;

		// This is a positional argument
		public HangFireJobAttribute(int minutes)
		{
			this.minutes = minutes;
		}

		public int Minutes
		{
			get { return this.minutes; }
		}
	}
}
