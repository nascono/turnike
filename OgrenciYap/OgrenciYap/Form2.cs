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
	public partial class Form2 : Form
	{
		public Ogrenci ogrenci;
		public Form2(Ogrenci ogrenci)
		{
			this.ogrenci = ogrenci;
			InitializeComponent();
		}

		private void Form2_Load(object sender, EventArgs e)
		{
			textBox1.Text = ogrenci.tam_ad;
			textBox2.Text = ogrenci.numara.ToString();
			textBox3.Text = ogrenci.kart_no;
			textBox4.Text = ogrenci.mezuniyet_tarihi.ToString();
			textBox5.Text = ogrenci.telno;
			radioButton2.Checked = true;
			radioButton1.Checked = ogrenci.izin;
			pictureBox1.Image = ogrenci.foto;
		}
		private void Button3_Click(object sender, EventArgs e)
		{
			ogrenci = new Ogrenci(textBox1.Text,Convert.ToInt32(textBox2.Text),textBox3.Text,Convert.ToInt32(textBox4.Text),pictureBox1.Image,radioButton1.Checked,textBox5.Text);
			this.Close();
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "";
			openFileDialog.ShowDialog();
			pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
			openFileDialog.Dispose();
		}
	}
}
