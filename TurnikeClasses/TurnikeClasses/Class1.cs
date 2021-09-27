using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace TurnikeClasses
{
	[Serializable()]
	public class Ogrenci
	{
		Seri seri = new Seri();
		public string tam_ad, kart_no;
		public Image foto;
		public int numara, mezuniyet_tarihi;
		public byte[] card_number_bytes = new byte[8];
		public byte[] card_number_with_footer_and_header = new byte[16];
		public bool izin = false;
		public string telno = "";
		public string konum = "";
		public Ogrenci(string tam_ad, int numara, string kart_no, int mezuniyet_tarihi, Image foto, bool izin, string telno)
		{
			this.tam_ad = tam_ad;
			this.numara = numara;
			this.kart_no = kart_no.ToUpper();
			this.mezuniyet_tarihi = mezuniyet_tarihi;
			this.foto = foto;
			this.izin = izin;
			this.telno = telno;
			card_number_bytes = Encoding.UTF8.GetBytes(kart_no);
			byte[] header = { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
			byte[] footter = { 0x20 };
			Array.Copy(header, 0, card_number_with_footer_and_header, 0, header.Length);
			Array.Copy(card_number_bytes, 0, card_number_with_footer_and_header, 7, card_number_bytes.Length);
			Array.Copy(footter, 0, card_number_with_footer_and_header, 15, footter.Length);
			if (!File.Exists("location.conf"))
			{
				File.Create("location.conf").Dispose();
			}
			StreamReader sr = new StreamReader("location.conf");
			konum = sr.ReadToEnd();
			sr.Close();
			sr.Dispose();
		}
		public Ogrenci()
		{
			if (!File.Exists("location.conf"))
			{
				File.Create("location.conf").Dispose();
			}
			StreamReader sr = new StreamReader("location.conf");
			konum = sr.ReadToEnd();
			sr.Close();
			sr.Dispose();
		}
		public Ogrenci(string gelen_konum)
		{
			if (!File.Exists("location.conf"))
			{
				File.Create("location.conf").Dispose();
			}
			File.WriteAllText("location.conf", gelen_konum + "\\");
			StreamReader sr = new StreamReader("location.conf");
			konum = sr.ReadToEnd();
			sr.Close();
			sr.Dispose();
		}

		public Ogrenci[] ogrencileri_getir()
		{
			if (File.Exists(konum+ "ogrenciler.ofas"))
			{
				StreamReader sr = new StreamReader(konum + "ogrenciler.ofas");
				byte[] arrBytes = Convert.FromBase64String(sr.ReadLine());
				sr.Close();
				sr.Dispose();
				Ogrenci[] obj = (Ogrenci[])seri.ByteArrayToObject(arrBytes);
				return obj;
			}

			return new Ogrenci[] { };
		}
		public void ogrencileri_kaydet(Ogrenci[] ogrenciler)
		{
			if (!File.Exists(konum+"ogrenciler.ofas")) { File.Create(konum+"ogrenciler.ofas").Dispose(); }
			StreamWriter sw = new StreamWriter(konum+"ogrenciler.ofas");
			sw.WriteLine(Convert.ToBase64String(seri.ObjectToByteArray(ogrenciler)));
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}


	}



	[Serializable()]
	public class Logger
	{
		Seri seri = new Seri();
		public string file_name = "";
		public string file_full_name = "";
		public Logger()
		{
			string file_name = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".log";
			string file_full_name = (new Ogrenci()).konum+"logs/" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".log";
			this.file_full_name = file_full_name;
			this.file_name = file_name;

			if (!Directory.Exists((new Ogrenci()).konum + "logs")) { Directory.CreateDirectory((new Ogrenci()).konum + "logs"); }

			if (!File.Exists(file_full_name))
			{
				File.Create(file_full_name).Dispose();
				StreamWriter sw = new StreamWriter(file_full_name);
				sw.WriteLine(Convert.ToBase64String(seri.ObjectToByteArray(new List<Log>())));
				sw.Flush();
				sw.Close();
				sw.Dispose();
			}

		}
		public Logger(string thefile)
		{
			file_full_name = thefile;
			file_name = thefile.Substring(thefile.LastIndexOf('\\') + 1);
		}
		public void new_log(Log my_log)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)seri.ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();

			loglar.Add(my_log);

			StreamWriter sw = new StreamWriter(file_full_name);
			sw.WriteLine(Convert.ToBase64String(seri.ObjectToByteArray(loglar)));
			sw.Flush();
			sw.Close();
			sw.Dispose();
			loglar.Clear();
		}


		public Log[] ogrencinin_loglarini_bul(Ogrenci ogrencim)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)seri.ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();
			return (from x in loglar where x.ogrenci == ogrencim select x).ToArray();
		}
		public Log[] olayin_loglarini_bul(string log)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)seri.ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();
			return (from x in loglar where x.olay == log select x).ToArray();
		}
		public Log[] tum_loglari_getir()
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)seri.ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();
			return loglar.ToArray();
		}
		public Log[] ogrencinin_olayli_loglarini_bul(Ogrenci ogrencim, string olayim)
		{
			StreamReader sr = new StreamReader(file_full_name);
			List<Log> loglar = (List<Log>)seri.ByteArrayToObject(Convert.FromBase64String(sr.ReadLine()));
			sr.Close();
			sr.Dispose();
			return (from x in loglar where x.ogrenci == ogrencim && x.olay == olayim select x).ToArray();
		}


	}








	[Serializable()]
	public class Log
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

	[Serializable()]
	public class Seri
	{
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
	}
}
