using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TurnikeClasses;

namespace OgrenciKartKayit
{
	public partial class Form2 : Form
	{
		int no = 0;
		Ogrenci[] ogrenciler = (new Ogrenci()).ogrencileri_getir();
		int ogrenci_indis = 0;
		public Form2(int no)
		{
			this.no = no;
			InitializeComponent();
		}

		private void Form2_Shown(object sender, EventArgs e)
		{
			Ogrenci[] bulunan_ogrenciler = (from x in ogrenciler where x.numara == no select x).ToArray();
			if (bulunan_ogrenciler.Length < 1)
			{
				MessageBox.Show("Numara Kayıtlı Değil");
				this.Close();
				return;
			}
			ogrenci_indis = Array.IndexOf(ogrenciler, bulunan_ogrenciler[0]);
			pictureBox1.Image = new Bitmap(ogrenciler[ogrenci_indis].foto,399,513);
			label4.Text = ogrenciler[ogrenci_indis].tam_ad;
			label5.Text = ogrenciler[ogrenci_indis].numara.ToString();
			textBox1.Focus();
		}

		private void TextBox1_TextChanged(object sender, EventArgs e)
		{
			
		}

		private void TextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				Ogrenci ogrencim = ogrenciler[ogrenci_indis];
				int girilen = 0;
				if (!int.TryParse(textBox1.Text,out girilen))
				{
					MessageBox.Show("Kart Yanlış");
					this.Close();
					return;
					
				}
				string hexValue = (girilen).ToString("X");

				hexValue = hexValue[6].ToString() + hexValue[7].ToString()
				+ hexValue[4].ToString() + hexValue[5].ToString()
				+ hexValue[2].ToString() + hexValue[3].ToString()
				+ hexValue[0].ToString() + hexValue[1].ToString();
				
				

				ogrenciler[ogrenci_indis] = new Ogrenci(ogrencim.tam_ad, ogrencim.numara, hexValue, ogrencim.mezuniyet_tarihi, ogrencim.foto, ogrencim.izin, ogrencim.telno);
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				this.Close();
			}
		}
	}
}
