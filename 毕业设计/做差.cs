using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
namespace 毕业设计
{
    public partial class 做差 : Form
    {
        public 做差()
        {
            InitializeComponent();
        }
        string[] filename;
        List<string> savefile = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == DialogResult.OK) 
            {
                //Bitmap bitmap = new Bitmap(openFile.FileName);
                //for (int i = 0; i < bitmap.Height; i++)
                //{
                //    for (int j = 0; j < bitmap.Width; j++)
                //    {
                //        Color color = Color.FromArgb(bitmap.GetPixel(j, i).R * 4, bitmap.GetPixel(j, i).G * 4, bitmap.GetPixel(j, i).B * 4);
                //        bitmap.SetPixel(j, i, color);
                //    }
                //}
                //bitmap.Save("res.png");
                filename =openFile.FileNames;
                foreach(string i in openFile.FileNames) 
                {
                    textBox1.Text += i;
                }
                

            }
        }
        string realfilename;
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();           
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                realfilename = openFile.FileName;
                textBox2.Text = openFile.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
             
            Bitmap realbitmap = new Bitmap(realfilename);

            int minx = realbitmap.Width, miny = realbitmap.Height;
            for (int i = 0; i < filename.Length; i++) 
            {

               

                Bitmap bitmap = new Bitmap(filename[i]);
                Color color = bitmap.GetPixel(258, 225);
                Bitmap save = new Bitmap(bitmap.Width, bitmap.Height,bitmap.PixelFormat);
                if (save.Width < minx && save.Height < miny)
                {
                    minx = save.Width;
                    miny = save.Height;
                }

                for (int j = 0; j < bitmap.Height; j++)
                {
                    for (int k = 0; k < bitmap.Width; k++)
                    {                      
                        Color r = bitmap.GetPixel(k, j);
                        Color g = realbitmap.GetPixel(k, j);
                        
                        Color b;
                        if (g == Color.FromArgb(255,0, 0, 0))
                        {
                            b = Color.FromArgb(255, 0, 0, 0);
                        }
                        else 
                        {
                            b = Color.FromArgb(255,Math.Abs(r.R - g.R), Math.Abs(r.G - g.G), Math.Abs(r.B - g.B));
                        }
                        
                        save.SetPixel(k, j, b);
                    }
                }
                save.Save( i + ".png");
                savefile.Add(i + ".png");
            }

            textBox3.Text = minx.ToString();
            textBox4.Text = miny.ToString();
            
        }
        public struct Data 
        {
            public int data;
            public int count;
        }
        struct Count 
        {
            public int[] data;
            public int[] pop;
            public int count;
            public int sum;
        }
        List<string> readfile = new List<string>();
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            if (open.ShowDialog() == DialogResult.OK) 
            {
                filename = open.FileNames;
                foreach (string i in open.FileNames)
                {
                    textBox5.Text += i;
                    readfile.Add(i);
                }
            }
            Count[] count = new Count[readfile.Count] ;

            SaveFileDialog save = new SaveFileDialog();
            if (save.ShowDialog() == DialogResult.OK) 
            { 
                StreamWriter streamWriter = new StreamWriter(save.FileName);
                for (int i = 0; i < readfile.Count; i++)
                {
                    Bitmap bitmap = new Bitmap(readfile[i]);
                    count[i].data = new int[256];
                    count[i].pop = new int[256];
                    count[i].sum = bitmap.Width * bitmap.Height;
                    count[i].count = 0;
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        for (int k = 0; k < bitmap.Width; k++)
                        {
                            Color color = bitmap.GetPixel(k, j);

                            if (color == Color.FromArgb(255,0, 0, 0))
                            {
                                count[i].count++;
                                count[i].data[(color.R + color.G + color.B) / 3]++;
                            }
                            else
                            {
                                count[i].data[(color.R + color.G + color.B) / 3]++;
                            }
                        }
                    }

                }
                double[,] pop = new double[1, filename.Length];
                
                for (int j = 0; j < filename.Length; j++)
                {
                    int sum=0;
                    for(int i = 0; i < Convert.ToInt32(textBox6.Text); i++) 
                    {
                        sum += count[j].data[i];
                    }
                    pop[0, j] = sum *1.0/ count[j].sum * 1.0;
                }
                             
                for (int j = 0; j < filename.Length; j++)
                {

                    streamWriter.WriteLine("窗口" + (j + 1) +  ":" + pop[0, j]);


                }

                
                streamWriter.Close();
            }
           

        }
    }
}
