using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BPUtil;
using BPUtil.Forms;

namespace LetsEncryptManager
{
	static class Program
	{
		static ServiceManager sm;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Globals.Initialize(exePath);

			if (Environment.UserInteractive)
			{
				string Title = "LetsEncryptManager " + LEGlobals.Version + " Service Manager";
				string ServiceName = "LetsEncryptManager";
				ButtonDefinition btnConfigure = new ButtonDefinition("Configure Service", btnConfigure_Click);
				ButtonDefinition btnExportKey = new ButtonDefinition("Pfx2crt&key", btnExportKey_Click);
				ButtonDefinition[] customButtons = new ButtonDefinition[] { btnConfigure, btnExportKey };

				Application.Run(sm = new ServiceManager(Title, ServiceName, customButtons));
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
					new MainService()
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
		static Form f = null;
		private static void btnConfigure_Click(object sender, EventArgs e)
		{
			if (f == null)
			{
				f = new ConfigurationForm();
				f.StartPosition = FormStartPosition.CenterParent;
				f.FormClosed += F_FormClosed;
				f.Show(sm);
			}
			else
				f.Focus();
		}

		private static void F_FormClosed(object sender, FormClosedEventArgs e)
		{
			f = null;
		}

		private static void btnExportKey_Click(object sender, EventArgs e)
		{
			CertificateExport.PfxToCrtKey("my.pfx", "N0t_V3ry-S3cure#lol", "test.crt", "test.key");
		}
	}
}
