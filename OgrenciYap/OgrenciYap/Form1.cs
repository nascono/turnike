using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TurnikeClasses;
namespace OgrenciYap
{
	public partial class Form1 : Form
	{
		Ogrenci[] ogrenciler;
		public Form1()
		{
			InitializeComponent();
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			ogrenciler = (new Ogrenci()).ogrencileri_getir();
			textBox4.Text = (DateTime.Now.Year + 4).ToString();
			yenile();
		}
		public void yenile()
		{
			listView1.Items.Clear();
			foreach (Ogrenci item in ogrenciler)
			{
				ListViewItem listViewItem = new ListViewItem(new string[] { item.tam_ad, item.numara.ToString(), item.kart_no, item.mezuniyet_tarihi.ToString(), item.izin.ToString(), item.telno });
				listView1.Items.Add(listViewItem);
			}
		}
		private void Button3_Click(object sender, EventArgs e)
		{
			try
			{
				if (pictureBox1.Image==null) { pictureBox1.Image = new Bitmap(133,171); }
				Ogrenci yeni_ogrenci = new Ogrenci(textBox1.Text, Convert.ToInt32(textBox2.Text), textBox3.Text, Convert.ToInt32(textBox4.Text), pictureBox1.Image, radioButton1.Checked, textBox5.Text);
				Ogrenci[] new_ogrenciler = new Ogrenci[ogrenciler.Length + 1];
				Array.Copy(ogrenciler, 0, new_ogrenciler, 0, ogrenciler.Length);
				new_ogrenciler[ogrenciler.Length] = yeni_ogrenci;
				ogrenciler = new_ogrenciler;
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				pictureBox1.Image = null;
				textBox1.Text = "";
				textBox2.Text = "";
				textBox3.Text = "";
				textBox4.Text = (DateTime.Now.Year + 4).ToString();
				textBox5.Text = "";
				radioButton2.Checked = true;
				yenile();
			}
			catch (Exception)
			{
				MessageBox.Show("HATA");
			}


		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				Form2 form2 = new Form2(ogrenciler[listView1.SelectedIndices[0]]);
				form2.ShowDialog();
				if (form2.degistimi)
				{
					ogrenciler[listView1.SelectedIndices[0]] = form2.ogrenci;
					(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
					yenile();
				}
				
			}
			
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "";
			openFileDialog.ShowDialog();
			if (openFileDialog.FileName =="")
			{
				return;
			}
			pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
			openFileDialog.Dispose();
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				List<Ogrenci> ogrencis = ogrenciler.ToList();
				ogrencis.Remove(ogrenciler[listView1.SelectedIndices[0]]);
				ogrenciler = ogrencis.ToArray();
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				yenile();
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			EminMisin eminMisin = new EminMisin();
			eminMisin.ShowDialog();
			if (eminMisin.onay)
			{
				List<Ogrenci> ogrencis = ogrenciler.ToList();
				Ogrenci[] silinecekler = (from x in ogrenciler where x.mezuniyet_tarihi == DateTime.Now.Year select x).ToArray();
				foreach (Ogrenci item in silinecekler)
				{
					ogrencis.Remove(item);
				}
				ogrenciler = ogrencis.ToArray();
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				yenile();
			}
			
		}

		private void Button6_Click(object sender, EventArgs e)
		{
			try
			{
				int atancakno = (from x in ogrenciler orderby x.numara select x).ToArray()[ogrenciler.Length-1].numara+1;
				Ogrenci yeni_ogrenci = new Ogrenci("PERSONEL", atancakno,"",3000,new Bitmap(133,171),true,"personel");
				Ogrenci[] new_ogrenciler = new Ogrenci[ogrenciler.Length + 1];
				Array.Copy(ogrenciler, 0, new_ogrenciler, 0, ogrenciler.Length);
				new_ogrenciler[ogrenciler.Length] = yeni_ogrenci;
				ogrenciler = new_ogrenciler;
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				pictureBox1.Image = null;
				textBox1.Text = "";
				textBox2.Text = "";
				textBox3.Text = "";
				textBox4.Text = (DateTime.Now.Year + 4).ToString();
				textBox5.Text = "";
				radioButton2.Checked = true;
				yenile();
			}
			catch (Exception)
			{
				MessageBox.Show("HATA");
			}
		}

		private void Button7_Click(object sender, EventArgs e)
		{
			try
			{
				int atancakno = (from x in ogrenciler orderby x.numara select x).ToArray()[ogrenciler.Length - 1].numara+1;
				Ogrenci yeni_ogrenci = new Ogrenci("Ziyaretçi", atancakno, "", 3000, new Bitmap(133, 171), true, "ziyaretci");
				Ogrenci[] new_ogrenciler = new Ogrenci[ogrenciler.Length + 1];
				Array.Copy(ogrenciler, 0, new_ogrenciler, 0, ogrenciler.Length);
				new_ogrenciler[ogrenciler.Length] = yeni_ogrenci;
				ogrenciler = new_ogrenciler;
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				pictureBox1.Image = null;
				textBox1.Text = "";
				textBox2.Text = "";
				textBox3.Text = "";
				textBox4.Text = (DateTime.Now.Year + 4).ToString();
				textBox5.Text = "";
				radioButton2.Checked = true;
				yenile();
			}
			catch (Exception)
			{
				MessageBox.Show("HATA");
			}
		}

		private void Button8_Click(object sender, EventArgs e)
		{
			Ogrenci[] telliler = (from x in ogrenciler where x.telno != "" select x).ToArray();
			Ogrenci[] telsizler = (from x in ogrenciler where x.telno == "" select x).ToArray();
			ogrenciler = telliler.Union(telsizler).ToArray();
			(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
			yenile();
		}

		private void Button9_Click(object sender, EventArgs e)
		{
			Ogrenci[] ogretmenler = (from x in ogrenciler where (x.telno != ""&& !x.telno.All(char.IsDigit)) select x).ToArray();
			Ogrenci[] ogrencilerrrr = (from x in ogrenciler where !(x.telno != "" && !x.telno.All(char.IsDigit))  select x).ToArray();
			ogrenciler = ogrencilerrrr.Union(ogretmenler).ToArray();
			(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
			yenile();
		}

		private void Button10_Click(object sender, EventArgs e)
		{
			ogrenciler = (from x in ogrenciler orderby x.numara select x).ToArray();
			(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
			yenile();
		}

		private void Button11_Click(object sender, EventArgs e)
		{
			ogrenciler = (from x in ogrenciler orderby x.tam_ad select x).ToArray();
			(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
			yenile();
		}

		private void Button12_Click(object sender, EventArgs e)
		{
			ogrenciler = (from x in ogrenciler orderby x.mezuniyet_tarihi select x).ToArray();
			(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
			yenile();
		}

		Image basilcak;
		private void Button13_Click(object sender, EventArgs e)
		{
			Font drawFont = new Font("Arial", 60);
			SolidBrush drawBrush = new SolidBrush(Color.Black);
			Image ogrenci_resim = ogrenciler[listView1.SelectedIndices[0]].foto;
			ogrenci_resim = new Bitmap(ogrenci_resim, 383, 500);
			Image kartt = new Bitmap(Properties.Resources.kimlik_karti);
			Graphics g = Graphics.FromImage(kartt);

			g.DrawString(ogrenciler[listView1.SelectedIndices[0]].tam_ad,drawFont,drawBrush,95,530);
			g.DrawString(ogrenciler[listView1.SelectedIndices[0]].numara.ToString(), drawFont, drawBrush, 95, 700);
			g.DrawImage(ogrenci_resim, 1141, 390);

			string temp_dosya = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".zip";
			File.WriteAllBytes(temp_dosya, Properties.Resources.wordkart);
			string temp_klasor = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString();
			System.IO.Compression.ZipFile.ExtractToDirectory(temp_dosya,temp_klasor);
			File.Delete(temp_klasor+ @"\word\media\image1.jpeg");
			kartt.Save(temp_klasor + @"\word\media\image1.jpeg", ImageFormat.Jpeg);
			System.IO.Compression.ZipFile.CreateFromDirectory(temp_klasor,temp_dosya+".docx");
			File.Delete(temp_dosya);
			Directory.Delete(temp_klasor,true);
			Process.Start(temp_dosya + ".docx");






			kartt.Save("asd.png");

		}
		public byte[] GetEmbeddedResource(string namespacename, string filename)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = namespacename + "." + filename;

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (MemoryStream reader = new MemoryStream())
			{
				stream.CopyTo(reader);
				return reader.ToArray();
			}
		}

		private void Button14_Click(object sender, EventArgs e)
		{
			Form3 form3 = new Form3();
			form3.ShowDialog();

		}
	}
}
