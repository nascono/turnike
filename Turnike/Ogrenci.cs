using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Turnike
{
	[Serializable]
	class Ogrenci
	{
		public string tam_ad, kart_no, foto;
		public int numara, mezuniyet_tarihi;
		public byte[] card_number_bytes = new byte[8];
		public byte[] card_number_with_footer_and_header = new byte[18];
		public bool izin = false;
		public string telno = "";
		public Ogrenci(string tam_ad, int numara, string kart_no, int mezuniyet_tarihi, string foto, bool izin,string telno)
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
			byte[] footter = { 0x20, 0x20, 0x20 };
			Array.Copy(header, 0, card_number_with_footer_and_header, 0, header.Length);
			Array.Copy(card_number_bytes, 0, card_number_with_footer_and_header, 7, card_number_bytes.Length);
			Array.Copy(footter, 0, card_number_with_footer_and_header, 15, footter.Length);

		}
		public Ogrenci() { }
		public Ogrenci[] ogrencileri_getir()
		{
			if (File.Exists("ogrenciler.ofas"))
			{
				StreamReader sr = new StreamReader("ogrenciler.ofas");
				byte[] arrBytes = Convert.FromBase64String(sr.ReadLine());
				MemoryStream memStream = new MemoryStream();
				BinaryFormatter binForm = new BinaryFormatter();
				memStream.Write(arrBytes, 0, arrBytes.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				Ogrenci[] obj = (Ogrenci[])binForm.Deserialize(memStream);
				return obj;
			}
			
			return new Ogrenci[] { };
		}
		

	}

}
