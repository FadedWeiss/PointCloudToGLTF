using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Numerics;
using SharpGLTF.IO;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;

namespace PointCloudToGLB
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		StringBuilder sb = new StringBuilder();
		int i1 = 1;

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e) //Операция открытия облака точек исходного
		{
			
		}

		private void button1_Click(object sender, EventArgs e) //Кнопка открыть исходное облако точек
		{
			if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;
		}

		private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e) //Каталог для сохранения
		{
			
		}

		public void Operations(StringBuilder sb, int i1, string SaveFolderPath)
		{
			var mesh = VertexBuilder<VertexPosition, VertexColor1, VertexEmpty>.CreateCompatibleMesh("points");
			var material = new MaterialBuilder();
			var model = ModelRoot.CreateModel();
			var guid = Guid.NewGuid();

			string DataStr = null;
			using (var reader = new StringReader(sb.ToString()))
			{
				while ((DataStr = reader.ReadLine()) != null)
				{
					string[] SingleStrValues = DataStr.Split(' ');
					float R_code = (float)Convert.ToDouble(SingleStrValues[3]);
					float G_code = (float)Convert.ToDouble(SingleStrValues[4]);
					float B_code = (float)Convert.ToDouble(SingleStrValues[5]);
					System.Numerics.Vector3 VectorOfPoint = new System.Numerics.Vector3((float)Convert.ToDouble(SingleStrValues[0]), (float)Convert.ToDouble(SingleStrValues[2]), (float)Convert.ToDouble(SingleStrValues[1]));

					mesh.UsePrimitive(material, 1).AddPoint((VectorOfPoint, new Vector4(R_code / 256f, G_code / 256f, B_code / 256f, 1)));

				}
			}
			model.CreateMeshes(mesh);
			model.UseScene("Default").CreateNode().WithMesh(model.LogicalMeshes[0]);
			if (listBox1.SelectedItem.ToString() == "GLTF") { model.SaveGLTF($@"{SaveFolderPath}\Export_Part-{i1}.gltf"); }
			else if (listBox1.SelectedItem.ToString() == "GLB") { model.SaveGLB($@"{SaveFolderPath}\Export_Part-{i1}.glb"); }
			//else if (listBox1.SelectedItem.ToString() == "OBJ") { model.SaveAsWavefront($@"{SaveFolderPath}\Export_Part-{i1}.obj"); }
			mesh = null;

		}

		private void button2_Click(object sender, EventArgs e) //Кнопка выбора папки сохранения
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel) return;
		}
		string SaveFolderPath = null;
		private void button3_Click(object sender, EventArgs e) //Кнопка запуска процесса
		{
			var guid = Guid.NewGuid();
			SaveFolderPath = folderBrowserDialog1.SelectedPath + @"\" + guid;
			System.IO.Directory.CreateDirectory(SaveFolderPath);
			

			//string FilePath = @"C:\Users\GeorgKeneberg\Documents\Temp\Region02.pts";
			long Counter = 0;

			foreach (var SingleStr in File.ReadLines(openFileDialog1.FileName))
			{
				sb.AppendLine(SingleStr);
				Counter++;

				if (Counter == 2000000)
				{
					Operations(sb, i1, SaveFolderPath);
					i1++;
					sb.Clear();
					Counter = 0;
				}

			}
			Operations(sb, i1, SaveFolderPath);
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //Выбор типа экспортируемого файла
		{

		}
	}
}
