using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LetsEncryptManager
{
	public static class LEGlobals
	{
		public static string Version
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Version.ToString();
			}
		}
	}
}
