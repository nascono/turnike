using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OgrenciKartKayit
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void TextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			

			if (e.KeyCode == Keys.Enter)
			{
				e.SuppressKeyPress = true;

				int no = 0;
				if (!int.TryParse(textBox1.Text, out no)) { textBox1.Clear(); return; }
				Form2 form2 = new Form2(no);
				form2.ShowDialog();

				textBox1.Clear();
			}
		}
	}
}
