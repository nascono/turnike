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
				ogrenciler[listView1.SelectedIndices[0]] = form2.ogrenci;
				(new Ogrenci()).ogrencileri_kaydet(ogrenciler);
				yenile();
			}
			
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "";
			openFileDialog.ShowDialog();
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
	}
}
