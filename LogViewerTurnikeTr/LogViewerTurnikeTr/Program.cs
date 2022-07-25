using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TurnikeClasses;

namespace LogViewerTurnikeTr
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1) { return; }
			Logger logger = new Logger(args[0]);
			string yazilcak = "";
			foreach (Log item in logger.tum_loglari_getir())
			{
				yazilcak += item.zaman.ToShortDateString() + ";" + item.zaman.ToShortTimeString() + ";" + item.ogrenci.numara + ";" + item.ogrenci.tam_ad + ";" + item.olay + "\n";
			}
			string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".csv";
			File.WriteAllText(fileName, yazilcak, Encoding.Default);
			Process.Start(fileName);
		}
	}
}
