using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Turnike
{
	class Logger
	{
		string file_name = "";
		string file_full_name = "";
		public Logger()
		{
			string file_name = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".log";
			string file_full_name = "logs/" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".log";
			this.file_full_name = file_full_name;
			this.file_name = file_name;
			if (!Directory.Exists("logs")) { Directory.CreateDirectory("logs"); }
			if (!File.Exists("logs/" + file_name))
			{
				File.Create(file_full_name).Dispose();
				StreamWriter sw = new StreamWriter(file_full_name);
				sw.WriteLine(Convert.ToBase64String(ObjectToByteArray(new List<Log>())));
				sw.Flush();
				sw.Close();
				sw.Dispose();
			}
			
		}
		public byte[] ObjectToByteArray(Object obj)
		{
			if (obj == null)
				return null;

			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream ms = new MemoryStream();
			bf.Serialize(ms, obj);

			return ms.ToArray();
		}

		// Convert a byte array to an Object
		public Object ByteArrayToObject(byte[] arrBytes)
		{
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter binForm = new BinaryFormatter();
			memStream.Write(arrBytes, 0, arrBytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			Object obj = (Object)binForm.Deserialize(memStream);

			return obj;
		}
		public void new_log(Log my_log)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();

			loglar.Add(my_log);

			StreamWriter sw = new StreamWriter(file_full_name);
			sw.WriteLine(Convert.ToBase64String(ObjectToByteArray(loglar)));
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
		public Log[] ogrencinin_loglarini_bul(Ogrenci ogrencim)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();
			return (from x in loglar where x.ogrenci == ogrencim select x).ToArray();
		}
		public Log[] ogrencinin_olayli_loglarini_bul(Ogrenci ogrencim,string olayim)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();
			return (from x in loglar where x.ogrenci == ogrencim && x.olay == olayim select x).ToArray();
		}
	}

	[Serializable]
	class Log
	{
		public Ogrenci ogrenci;
		public string olay;
		public DateTime zaman;
		public Log(Ogrenci ogrenci, string olay)
		{
			this.ogrenci = ogrenci;
			this.olay = olay;
			this.zaman = DateTime.Now;
		}

	}
}
