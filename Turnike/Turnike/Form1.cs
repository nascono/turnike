﻿using System;
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
					sonokunanlar.Add(item, "");
					girisportlari.Add(new SerialPort(item, 9600));
					girisportlari[girisportlari.Count - 1].Open();
					girisportlari[girisportlari.Count - 1].DataReceived += giris_geldi;
				}
				
			}
			foreach (string item in cikis_portlari_txts)
			{
				if (item != "")
				{
					sonokunanlar.Add(item, "");
					cikisportlari.Add(new SerialPort(item, 9600));
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
			
			SerialPort sp = sender as SerialPort;
			byte[] gelen;
			gelen = new byte[sp.BytesToRead];
			sp.Read(gelen, 0, sp.BytesToRead);


			if (gelen.Length>4)
			{
				if (!gelen.SequenceEqual(new byte[] { 0x45, 0x52, 0x52, 0x01, 0x02, 0x0D }) && gelen.Length == 16 && ByteArrayToString(gelen) != sonokunanlar[sp.PortName])
				{
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
						}
					}
					else
					{

						listBox1.Items.Add(new object[] { new Ogrenci("Tanımsız Kart \n" + ByteArrayToString(ogenciler[0].card_number_with_footer_and_header) + "\n" + ByteArrayToString(gelen) + "\n" + gelen.SequenceEqual(ogenciler[0].card_number_with_footer_and_header).ToString(), 0, "", 0, null, false, "asd"), Color.Red, "" });
					}
				}
				sonokunanlar[sp.PortName] = ByteArrayToString(gelen);
			}
			
			

		}

		string bir_onceki_cikis = "";
		private void cikis_geldi(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			SerialPort sp = sender as SerialPort;
			byte[] gelen = new byte[sp.BytesToRead];
			sp.Read(gelen, 0, sp.BytesToRead);
			if (gelen.Length>4)
			{
				if (!gelen.SequenceEqual(new byte[] { 0x45, 0x52, 0x52, 0x01, 0x02, 0x0D }) && gelen.Length == 16 && ByteArrayToString(gelen) != sonokunanlar[sp.PortName])
				{
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
							else if(bir_sonraki_idari_izinli)
							{
								bir_sonraki_idari_izinli = false;
								logger.new_log(new Log(ogrencim, "İzinli Çıkış Yaptı(İdareden İzinli)"));
								sp.Write(new byte[] { 0x52, 0xC8 }, 0, 2);
								listBox1.Items.Add(new object[] { ogrencim, Color.LightGreen, "İzinli Çıkış Yaptı(İdareden İzinli)" });
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
					sonokunanlar[sp.PortName] = ByteArrayToString(gelen);
				}
			}
			

		}
		void sleep() 
		{
			long baslangic = DateTime.Now.Ticks;
			while (DateTime.Now.Ticks-baslangic<TimeSpan.TicksPerSecond*1)
			{
				Application.DoEvents();
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
				bir_sonraki_idari_izinli = true;
				Thread th = new Thread(new ThreadStart(bir_sonraki_idari_izinli_sil));
				th.Start();
			}
			
		}
		void bir_sonraki_idari_izinli_sil()
		{
			Thread.Sleep(10000);
			bir_sonraki_idari_izinli = false;
		}

	}
}
