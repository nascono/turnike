using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Runtime.Serialization.Formatters.Binary;
using TurnikeClasses;
using System.Threading;
using System.Net.Http;
using System.Web;
namespace Turnike
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			
		}
		Logger logger = new Logger();
		private float PictureHeight = 150f;
		private int ItemMargin = 5;
		List<SerialPort> girisportlari = new List<SerialPort>();
		List<SerialPort> cikisportlari = new List<SerialPort>();
		int giris_saati = 10;
		int giris_dakikasi = 0;
		int cikis_saati = 15;
		int cikis_dakikasi = 0;
		bool bir_sonraki_idari_izinli = false;

		Ogrenci[] ogenciler;

		
		
		private void ListBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			
			
			ListBox lst = sender as ListBox;

			if (e.Index == -1) { return; }
			Ogrenci ogrencim = (Ogrenci)((object[])lst.Items[e.Index])[0];
			Color renk = (Color)((object[])lst.Items[e.Index])[1];
			string mesaj = (string)((object[])lst.Items[e.Index])[2];
			Image foto = new Bitmap(133,171);
			Graphics g = Graphics.FromImage(foto);
			g.FillRectangle(new SolidBrush(Color.Black), 0, 0, foto.Width, foto.Height);
			if (ogrencim.foto != null)
			{
				foto = ogrencim.foto;
			}
			
			// Draw the background.
			e.DrawBackground();

			// Draw the picture.
			float scale = PictureHeight / foto.Height;
			RectangleF source_rect = new RectangleF(0, 0, foto.Width, foto.Height);
			float picture_width = scale * foto.Width;
			RectangleF dest_rect = new RectangleF(e.Bounds.Left + ItemMargin, e.Bounds.Top + ItemMargin, picture_width, PictureHeight);
			e.Graphics.DrawImage(foto, dest_rect, source_rect, GraphicsUnit.Pixel);

			// See if the item is selected.

			// Find the area in which to put the text.
			float x = e.Bounds.Left + picture_width + 3 * ItemMargin;
			float y = e.Bounds.Top + ItemMargin;
			float width = e.Bounds.Right - ItemMargin - x;
			float height = e.Bounds.Bottom - ItemMargin - y;
			RectangleF layout_rect = new RectangleF(x, y, width, height);


			string txt = ogrencim.tam_ad + '\n' + ogrencim.numara + '\n' + mesaj;

			e.Graphics.FillRectangle(new SolidBrush(renk), Rectangle.Round(layout_rect));
			e.Graphics.DrawString(txt, new Font("Times New Roman", 24f), new SolidBrush(e.ForeColor), layout_rect);
			if (lst.Items.Count == 5)
			{
				lst.Items.RemoveAt(0);
			}
		}

		private void ListBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = (int)(PictureHeight + 2 * ItemMargin);
		}

		Dictionary<string, string> sonokunanlar = new Dictionary<string, string>();

		private void Form1_Load(object sender, EventArgs e)
		{
			listBox1.Focus();
			StreamReader sr = new StreamReader("giris_portlari.txt");
			string[] giris_portlari_txts = sr.ReadToEnd().Split(';');
			sr.Close();
			sr.Dispose();
			StreamReader srr = new StreamReader("cikis_portlari.txt");
			string[] cikis_portlari_txts = srr.ReadToEnd().Split(';');
			srr.Close();
			srr.Dispose();
			if (!File.Exists("saatler.txt")){ File.Create("saatler.txt").Dispose(); File.WriteAllText("saatler.txt", "10:00@15:00"); }
			string saatlertxt = File.ReadAllText("saatler.txt");
			giris_saati = Convert.ToInt32(saatlertxt.Split('@')[0].Split(':')[0]);
			giris_dakikasi = Convert.ToInt32(saatlertxt.Split('@')[0].Split(':')[1]);
			cikis_saati = Convert.ToInt32(saatlertxt.Split('@')[1].Split(':')[0]);
			cikis_dakikasi = Convert.ToInt32(saatlertxt.Split('@')[1].Split(':')[1]);

			foreach (string item in giris_portlari_txts)
			{
				if (item != "")
				{
					
					girisportlari.Add(new SerialPort(item, 9600));
					sonokunanlar.Add(girisportlari[girisportlari.Count - 1].PortName, "");
					girisportlari[girisportlari.Count - 1].Open();
					girisportlari[girisportlari.Count - 1].DataReceived += giris_geldi;
					
				}
				
			}
			foreach (string item in cikis_portlari_txts)
			{
				if (item != "")
				{
					
					cikisportlari.Add(new SerialPort(item, 9600));
					sonokunanlar.Add(cikisportlari[cikisportlari.Count - 1].PortName, "");
					cikisportlari[cikisportlari.Count - 1].Open();
					cikisportlari[cikisportlari.Count - 1].DataReceived += cikis_geldi;
					
				}
				
			}
			ogenciler = new Ogrenci().ogrencileri_getir();


			listBox1.Width = this.Width - 50;
			listBox1.Height = this.Height - 10;
		}


		
		private void giris_geldi(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			try
			{
				SerialPort sp = sender as SerialPort;
				byte[] gelen;
				gelen = new byte[sp.BytesToRead];
				sp.Read(gelen, 0, sp.BytesToRead);


				if (gelen.Length > 4)
				{
					if (!gelen.SequenceEqual(new byte[] { 0x45, 0x52, 0x52, 0x01, 0x02, 0x0D }) && gelen.Length == 16 && ByteArrayToString(gelen) != sonokunanlar[sp.PortName])
					{
						sonokunanlar[sp.PortName] = ByteArrayToString(gelen);
						Ogrenci[] bulunanlar = (from x in ogenciler where x.card_number_with_footer_and_header.SequenceEqual(gelen) select x).ToArray();
						if (bulunanlar.Length > 0)
						{
							Ogrenci ogrencim = bulunanlar[0];
							DateTime gec_zamani = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, giris_saati, giris_dakikasi, 0, 0);
							if (DateTime.Compare(DateTime.Now, gec_zamani) < 0)
							{
								logger.new_log(new Log(ogrencim, "Giriş Yapıldı"));
								sp.Write(new byte[] { 0x52, 0xC8 }, 0, 2);
								listBox1.Items.Add(new object[] { ogrencim, Color.Green, "Giriş Yapıldı" });
							}
							else
							{
								logger.new_log(new Log(ogrencim, "Geç Giriş Yapıldı"));
								sp.Write(new byte[] { 0x52, 0xC8 }, 0, 2);
								listBox1.Items.Add(new object[] { ogrencim, Color.DarkGreen, "Geç Giriş Yapıldı" });
								gec_giris_sms_gonder(ogrencim);
							}
						}
						else
						{
							listBox1.Items.Add(new object[] { new Ogrenci("Tanımsız Kart \n" + ByteArrayToString(gelen) + "\n" + ByteArrayToString(gelen) + "\n" + gelen.SequenceEqual(gelen).ToString(), 0, "", 0, null, false, "asd"), Color.Red, "" });
						}
					}

				}
			}
			catch
			{

			}
			
			
			

		}

		string bir_onceki_cikis = "";
		private void cikis_geldi(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			try
			{
				SerialPort sp = sender as SerialPort;
				byte[] gelen = new byte[sp.BytesToRead];
				sp.Read(gelen, 0, sp.BytesToRead);
				if (gelen.Length > 4)
				{
					if (!gelen.SequenceEqual(new byte[] { 0x45, 0x52, 0x52, 0x01, 0x02, 0x0D }) && gelen.Length == 16 && ByteArrayToString(gelen) != sonokunanlar[sp.PortName])
					{
						sonokunanlar[sp.PortName] = ByteArrayToString(gelen);
						Ogrenci[] bulunanlar = (from x in ogenciler where x.card_number_with_footer_and_header.SequenceEqual(gelen) select x).ToArray();
						if (bulunanlar.Length > 0)
						{
							Ogrenci ogrencim = bulunanlar[0];
							DateTime cikis_zamani = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, cikis_saati, cikis_dakikasi, 0, 0);
							if (DateTime.Compare(DateTime.Now, cikis_zamani) >= 0)
							{
								logger.new_log(new Log(ogrencim, "Çıkış Yapıldı"));
								sp.Write(new byte[] { 0x52, 0xC8 }, 0, 2);
								listBox1.Items.Add(new object[] { ogrencim, Color.Green, "Çıkış Yapıldı" });
							}
							else
							{
								if (ogrencim.izin)
								{
									logger.new_log(new Log(ogrencim, "İzinli Çıkış Yaptı(Yetkili)"));
									sp.Write(new byte[] { 0x52, 0xC8 }, 0, 2);
									listBox1.Items.Add(new object[] { ogrencim, Color.LightGreen, "İzinli Çıkış Yaptı(Yetkili)" });
								}
								else if (bir_sonraki_idari_izinli)
								{
									bir_sonraki_idari_izinli = false;
									logger.new_log(new Log(ogrencim, "İzinli Çıkış Yaptı(İdareden İzinli)"));
									sp.Write(new byte[] { 0x52, 0xC8 }, 0, 2);
									listBox1.Items.Add(new object[] { ogrencim, Color.LightGreen, "İzinli Çıkış Yaptı(İdareden İzinli)" });
									idari_izinli_sms_gonder(ogrencim);
								}
								else
								{
									logger.new_log(new Log(ogrencim, "Çıkmaya Çalıştı Ama Çıkamadı"));
									listBox1.Items.Add(new object[] { ogrencim, Color.Red, "İzin Yok" });
								}
							}

						}
						else
						{
							listBox1.Items.Add(new object[] { new Ogrenci("Tanımsız Kart", 0, "", 0, null, false, "asd"), Color.Red, "", "asd" });
						}

					}
				}
			}
			catch 
			{

			}
			
			

		}

		private void Timer1_Tick(object sender, EventArgs e)
		{
			byte[] yazilcak = {0x02,0xff,0x96,0x04,0x0b,0x36,0x34,0x03};
			foreach (SerialPort item in girisportlari)
			{
				item.Write(yazilcak, 0, yazilcak.Length);
			}
			foreach (SerialPort item in cikisportlari)
			{
				item.Write(yazilcak, 0, yazilcak.Length);
			}
		}


		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		private void ListBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.D1)
			{
				son_girisleri_sil();
				bir_sonraki_idari_izinli = true;
				
				Thread th = new Thread(new ThreadStart(bir_sonraki_idari_izinli_sil));
				th.Start();
			}
			if (e.KeyCode == Keys.D3)
			{
				Form2 form2 = new Form2();
				form2.ShowDialog();
				int gelenno = 0;
				if (!int.TryParse(form2.no, out gelenno))
				{
					return;
				}
				Ogrenci[] bulunanlar = (from x in ogenciler where x.numara==gelenno select x).ToArray();

				if (bulunanlar.Length > 0)
				{
					Ogrenci ogrencim = bulunanlar[0];
					DateTime gec_zamani = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, giris_saati, giris_dakikasi, 0, 0);
					if (DateTime.Compare(DateTime.Now, gec_zamani) < 0)
					{
						logger.new_log(new Log(ogrencim, "Giriş Yapıldı"));
						girisportlari[0].Write(new byte[] { 0x52, 0xC8 }, 0, 2);
						listBox1.Items.Add(new object[] { ogrencim, Color.Green, "Giriş Yapıldı" });
					}
					else
					{
						logger.new_log(new Log(ogrencim, "Geç Giriş Yapıldı"));
						girisportlari[0].Write(new byte[] { 0x52, 0xC8 }, 0, 2);
						listBox1.Items.Add(new object[] { ogrencim, Color.DarkGreen, "Geç Giriş Yapıldı" });
						gec_giris_sms_gonder(ogrencim);
					}
				}

			}
			if (e.KeyCode==Keys.D8)
			{
				son_girisleri_sil();
				girisportlari[0].Write(new byte[] { 0x52, 0xC8 }, 0, 2);
			}
			if (e.KeyCode == Keys.D9)
			{
				son_girisleri_sil();
				cikisportlari[0].Write(new byte[] { 0x52, 0xC8 }, 0, 2);
			}

		}
		void son_girisleri_sil()
		{
			List<string> keyList = new List<string>(sonokunanlar.Keys);
			foreach (string item in keyList)
			{
				sonokunanlar[item] = "";
			}
		}
		void bir_sonraki_idari_izinli_sil()
		{
			Thread.Sleep(10000);
			bir_sonraki_idari_izinli = false;
		}

		private void Timer2_Tick(object sender, EventArgs e)
		{
			//gelmeyen sms
			if (DateTime.Now.Hour == giris_saati && DateTime.Now.Minute == giris_dakikasi)
			{
				Ogrenci[] gelenler = (from x in logger.olayin_loglarini_bul("Giriş Yapıldı") select x.ogrenci).ToArray();
				int[] gelenler_nolar = (from x in gelenler select x.numara).ToArray();
				Ogrenci[] gelmeyenler = (from x in ogenciler where !gelenler_nolar.Contains(x.numara) select x).ToArray();
				Thread gelmeyenler_smslerini_gonder_th = new Thread(() => gelmeyenler_smslerini_gonder(gelmeyenler));
				gelmeyenler_smslerini_gonder_th.Start();
			}
		}
		void gelmeyenler_smslerini_gonder(Ogrenci[] gec_gelenler)
		{
			foreach (var item in gec_gelenler)
			{
				string telno = item.telno;
				if (telno !="" && Char.IsDigit(telno[0]))
				{
					string mesaj = "Öğrenciniz " + item.tam_ad + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " tarihi ile okula giriş yapmamış bulunmaktadır. Bilgilerinize arz olunur. Çamlıca Anadolu Lisesi Müdürlüğü";
					sms(telno, mesaj);

				}
			}
		}
		void gec_giris_sms_gonder(Ogrenci gec_giren)
		{
			string telno = gec_giren.telno;
			if (telno != "" && Char.IsDigit(telno[0]))
			{
				string mesaj = "Öğrenciniz " + gec_giren.tam_ad + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " tarihi ile okula geç giriş yapmıştır. Bilgilerinize arz olunur. Çamlıca Anadolu Lisesi Müdürlüğü";
				sms(telno, mesaj);
			}
		}
		void idari_izinli_sms_gonder(Ogrenci izinli)
		{
			string telno = izinli.telno;
			if (telno != "" && Char.IsDigit(telno[0]))
			{
				string mesaj = "Öğrenciniz " + izinli.tam_ad + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " tarihi ile okuldan idari izinle çıkış yapmıştır. Bilgilerinize arz olunur. Çamlıca Anadolu Lisesi Müdürlüğü";
				sms(telno, mesaj);
			}
		}
		async void sms(string telno,string mesaj)
		{
			try
			{
				HttpClient client = new HttpClient();
				string theurl = "https://api.iletimerkezi.com/v1/send-sms/get/?username=5375681048&password=974068&text=" + mesaj + "&receipents=" + telno.ToString() + "&sender=CAMLICAANDL";
				//string urll = "https://smartaudiogetter.com/smsdeneme.php?no="+telno+"&mesaj="+mesaj;
				await client.GetStringAsync(theurl);
				client.Dispose();
			}
			catch 
			{

			}
			
		}

	}
}
