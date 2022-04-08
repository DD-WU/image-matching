using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace 毕业设计
{
    public partial class 图片列表 : Form
    {    
        public 图片列表()
        {
            InitializeComponent();
            
        }

        string SavePath= "E:\\原始数据\\";//灰度图存储地
        static int MaxSize = 25;//图片数
        int count = 0;//窗口数
        List<Form1> forms = new List<Form1>();//窗口数组
        string[] path = new string[MaxSize];//图片读取路径
        int top = 0;
        DataSet set;
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
           
            
            if (open.ShowDialog() == DialogResult.OK)
            { }
            else
                return;
            foreach (string str in open.FileNames)
            {
                path[top++] = str;
            }
            //Bitmap[] bitmap;
            //bitmap = new Bitmap[top];
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            treeView1.Nodes.Clear();
            for (int i = 0; i < top; i++)
            {
                //bitmap[i] = new Bitmap(path[i]);
                //bitmaps.Add(bitmap[i]);
                treeView1.Nodes.Add(path[i]);              
            }

        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) 
            {
                if (e.Node.Level == 0)
                {
                    this.添加数据ToolStripMenuItem.Visible = false;
                    this.显示ToolStripMenuItem.Visible = true;
                    treeView1.SelectedNode = e.Node;
                }
            }
            
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void 图片列表_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
           
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                if (sender == treeView1)
                {
                    this.treeView1.SelectedNode = null;
                    this.显示ToolStripMenuItem.Visible = false;
                    this.添加数据ToolStripMenuItem.Visible = true;
                }         
            }
            
        }

        private void 添加数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click_1(sender, e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {

                comboBox1.Items.Add("display" + count);
                Form1 form = new Form1();
                form.Name = "display" + count;
                form.Text = "display" + count;
                form.remove += Form_remove;
                forms.Add(form);
                count++;            
                forms[forms.Count() - 1].Show();
                
            }
            else 
            {
                forms.Find(s=>s.Name==(string)comboBox1.SelectedItem).Show();
            }
        }

        private void Form_remove(string formName)
        {
            int i = Convert.ToInt32(formName.Replace("display", ""));       
            comboBox1.Items.Remove(formName);           
        }

       
        private Bitmap toGray(Bitmap curBitmap) 
        {
            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
            //以可读写的方式锁定全部位图像素
            System.Drawing.Imaging.BitmapData bmpData = curBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, curBitmap.PixelFormat);
            //得到首地址
            IntPtr ptr = bmpData.Scan0;
            //24位bmp位图字节数
            int bytes = bmpData.Stride * bmpData.Height;
            //定义位图数组
            byte[] rgbValues = new byte[bytes];
            //复制被锁定的位图像值到该数组内
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            //灰度化
            double colorTemp = 0;
            for (int i = 0; i < bmpData.Height; i++)
            {
                //利用公式（2.2）计算灰度值,只处理每行中是图像像素的数据，舍弃未用空间
                for (int j = 0; j < bmpData.Width * 3; j += 3)
                {
                    colorTemp = rgbValues[i * bmpData.Stride + j + 2] *0.114  + rgbValues[i * bmpData.Stride + j + 1] * 0.587 + rgbValues[i * bmpData.Stride + j] * 0.299;//之需改动此公式就可以改变灰度化图像
                                                                                                                                                                          //R=G=B
                    rgbValues[i * bmpData.Stride + j + 2] = rgbValues[i * bmpData.Stride + j + 1] = rgbValues[i * bmpData.Stride + j] = (byte)colorTemp;
                }

            }
            //把数组复制回位图
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            //解锁位图像素
            curBitmap.UnlockBits(bmpData);
            return curBitmap;
        }
        int graynumber = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (RGB.Checked == true)
            {
                FileStream fileStream = new FileStream(treeView1.SelectedNode.Text, FileMode.Open, FileAccess.Read);
                forms[comboBox1.SelectedIndex - 1].pictureBox1.Image = Image.FromStream(fileStream,true);
                fileStream.Close();
                fileStream.Dispose();               
                //forms[comboBox1.SelectedIndex - 1].pictureBox1.Image = Image.FromFile(path[treeView1.SelectedNode.Index]);
            }
            else if (Gray.Checked == true) 
            {
                Bitmap bitmap1 = new Bitmap(path[treeView1.SelectedNode.Index]);            
                forms[comboBox1.SelectedIndex - 1].pictureBox1.Image = toGray(bitmap1);
                graynumber++;        
            }


        }

        private void 图片列表_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        private void Gray_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
