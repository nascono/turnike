using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Turnike
{
	public partial class Form2 : Form
	{
		public string no = "";
		public Form2()
		{
			InitializeComponent();
			textBox1.Focus();
		}

		private void TextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode==Keys.Enter)
			{
				no = textBox1.Text;
				this.Close();
			}
		}
	}
}
