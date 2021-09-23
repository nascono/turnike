using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PdfToImages;
using TurnikeClasses;

namespace OgrenciOlusuturucu
{
	class Program
	{
		static int resimno = 1;
		static string konum = "";
		static string silincek = "139abeca06d1dfb6d5925517f00db8c8";
		static List<Ogrenci> ogrencis = new List<Ogrenci>();
		static Image beyaz;
		static PdfToImage pdfconvertor = new PdfToImage();
		static void Main(string[] args)
		{
			Bitmap b = new Bitmap(1, 1);
			b.SetPixel(0, 0, Color.White);
			beyaz = new Bitmap(b, 133, 171);
			konum = Console.ReadLine() + "\\";
			ekle(ConvertExcelToCsv(konum + "9.xls"),DateTime.Now.Year+4);
			ekle(ConvertExcelToCsv(konum + "10.xls"), DateTime.Now.Year + 3);
			ekle(ConvertExcelToCsv(konum + "11.xls"), DateTime.Now.Year + 2);
			ekle(ConvertExcelToCsv(konum + "12.xls"), DateTime.Now.Year + 1);
			Directory.CreateDirectory(konum + "resimler");
			ExtractAllImages(konum+"9.pdf");
			ExtractAllImages(konum + "10.pdf");
			ExtractAllImages(konum + "11.pdf");
			ExtractAllImages(konum + "12.pdf");
			Console.WriteLine((ogrencis.Count- Directory.GetFiles(konum + "resimler").Length).ToString()+ " adet ogrencin fotosu yok.");
			while (ogrencis.Count != Directory.GetFiles(konum + "resimler").Length)
			{
				Console.Write("Fotosu Olmayan Öğrenci: ");
				int tasincakno = Convert.ToInt32(Console.ReadLine());
				Ogrenci tasincak = (from x in ogrencis where x.numara == tasincakno select x).ToArray()[0];
				ogrencis.Remove(tasincak);
				ogrencis.Add(tasincak);
				beyaz.Save(konum + "resimler\\" + resimno.ToString() + ".jpg");
				resimno++;
			}
			for (int i = 0; i < ogrencis.Count; i++)
			{
				ogrencis[i].foto = Image.FromFile(konum+"resimler\\" + (i+1).ToString() + ".jpg");
			}

			string cikti = "";
			for (int i = 0; i < ogrencis.Count; i++)
			{
				cikti += (i + 1).ToString() + ";" + ogrencis[i].tam_ad + ";" + ogrencis[i].numara + "\n";
			}
			File.WriteAllText(konum + "basilcakkartlar.csv", cikti);
			Ogrenci kayt = new Ogrenci();
			kayt.ogrencileri_kaydet(ogrencis.ToArray());
			Console.ReadLine();
		}
		static void ekle(string intext, int mezuniyet)
		{
			string[] og = intext.Split('\n');
			og = (from x in og where x.Length > 0 && Char.IsDigit(x[0]) select x).ToArray();
			foreach (string item in og)
			{
				string[] ogs = item.Split(',');
				string tamad = ogs[3] + " " + ogs[8];
				int no = Convert.ToInt32(ogs[1]);
				ogrencis.Add(new Ogrenci(tamad,no,"",mezuniyet, beyaz,false,""));
			}
		}

		static string[] ayir(string asd) { return asd.Split(','); }
		static void ExtractAllImages(string dosya)
		{
			Bitmap[] resimler = pdfconvertor.pdftoimages(dosya);
			for (int i = 0; i < resimler.Length; i++)
			{
				resimler[i].Save(konum + "resimler\\" + resimno.ToString() + ".jpg", ImageFormat.Jpeg);
				if (CalculateMD5(konum + "resimler\\" + resimno.ToString() + ".jpg") == silincek)
				{
					File.Delete(konum + "resimler\\" + resimno.ToString() + ".jpg");
				}
				else
				{
					resimno++;
				}

			}
		}
		static string CalculateMD5(string filename)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(filename))
				{
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}

		static string ConvertExcelToCsv(string excelFilePath, int worksheetNumber = 1)
		{
			if (!File.Exists(excelFilePath)) throw new FileNotFoundException(excelFilePath);

			// connection string
			var cnnStr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;IMEX=1;HDR=NO\"", excelFilePath);
			var cnn = new OleDbConnection(cnnStr);

			// get schema, then data
			var dt = new DataTable();
			try
			{
				cnn.Open();
				var schemaTable = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
				if (schemaTable.Rows.Count < worksheetNumber) throw new ArgumentException("The worksheet number provided cannot be found in the spreadsheet");
				string worksheet = schemaTable.Rows[worksheetNumber - 1]["table_name"].ToString().Replace("'", "");
				string sql = String.Format("select * from [{0}]", worksheet);
				var da = new OleDbDataAdapter(sql, cnn);
				da.Fill(dt);
			}
			catch (Exception e)
			{
				// ???
				throw e;
			}
			finally
			{
				// free resources
				cnn.Close();
			}

			// write out CSV data
			string veri = "";
			foreach (DataRow row in dt.Rows)
			{
				bool firstLine = true;
				foreach (DataColumn col in dt.Columns)
				{
					if (!firstLine) {veri += ","; } else { firstLine = false; }
					var data = row[col.ColumnName].ToString().Replace("\"", "\"\"");
					veri += data;
				}
				veri += "\n";
			}
			return veri;
		}

	}
}
