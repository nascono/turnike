using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OgrenciYap
{
	public partial class EminMisin : Form
	{
		public bool onay = false;
		public EminMisin()
		{
			InitializeComponent();
			
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			onay = true;
			this.Close();
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			onay = false;
			this.Close();
		}
	}
}
