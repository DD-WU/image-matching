using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace 毕业设计
{

    public partial class Form1 : Form
    {

        public delegate void Removethis(string formName);
        public event Removethis remove;
        DataTable table;int count;
        public void txt2dataTable(string path, int n)
        {
            StreamReader stream = new StreamReader(path, System.Text.Encoding.Default);
            string name = stream.ReadLine();
            string[] readerofColumnNumber = name.Split(',');
            foreach (var i in readerofColumnNumber)
            {
                DataColumn col = table.Columns.Add(i.ToString());
            }
            string nextLine;
            while ((nextLine = stream.ReadLine()) != null)
            {
                string[] every_row = nextLine.Split(',');
                DataRow dr = table.NewRow();
                count = table.Columns.Count;
                for (int i = 0; i < count; i++)
                {
                    dr[i] = every_row[i];
                }
                table.Rows.Add(dr);
            }

        }
        public Form1()
        {
            InitializeComponent();
            
        } 
        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        public void 移除窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            remove(this.Text);
            this.Dispose();
            this.Close();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == DialogResult.OK) 
            {
                this.pictureBox1.Image.Save(saveFile.FileName);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK) 
            {
                table = new DataTable();
                txt2dataTable(openFile.FileName, 0);
                textBox1.Text = openFile.FileName;
            }
            //SaveFileDialog saveFile = new SaveFileDialog();
            //if (saveFile.ShowDialog()==DialogResult.OK)
            //{
                
            //    StreamWriter writer = new StreamWriter(saveFile.FileName);
            //    Bitmap bitmap = new Bitmap(pictureBox1.Image);
            //    Color b= bitmap.GetPixel(368, 87);
            //    for (int i = 0; i < table.Rows.Count; i++) 
            //    {
            //        writer.WriteLine(Convert.ToString(bitmap.GetPixel(Convert.ToInt32(table.Rows[i][1]), Convert.ToInt32(table.Rows[i][2])).R));
            //    }
            //    writer.Flush();
            //    writer.Close();
            //}
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            if (radioButton1.Checked)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    g.FillEllipse(new SolidBrush(Color.Blue), Convert.ToInt32(table.Rows[i][1]), Convert.ToInt32(table.Rows[i][2]), 5, 5);
                }
            }
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    g.FillEllipse(new SolidBrush(Color.Blue), Convert.ToInt32(table.Rows[i][3]), Convert.ToInt32(table.Rows[i][4]), 5, 5);
                }
            }
        }
    }
}
