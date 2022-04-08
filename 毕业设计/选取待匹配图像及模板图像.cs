using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 毕业设计
{
    public partial class 选取待匹配图像及模板图像 : Form
    {
        string Mode;
        public 选取待匹配图像及模板图像(string mode)
        {
            InitializeComponent();
            Mode = mode;
        }
        DataSet Data=new DataSet();
        int count;
        byte[,] det_gray, src_gray;
        int det_height, det_width,det_stride;
        int src_height, src_width, src_stride;
        Form1 form1, form2;
        double cd = 0, ef = 0, gh = 0;
        double Entropy(byte[,] img,int height,int width)
        {

            //开辟内存
            double[] temp = new double[256] ;

            // 计算每个像素的累积值
            for (int m = 0; m < height; m++)
            {// 有效访问行列的方式
                
                for (int n = 0; n < width; n++)
                {
                    int i = img[m,n];
                    temp[i] = temp[i] + 1;
                }
            }

            // 计算每个像素的概率
            for (int i = 0; i < 256; i++)
            {
                temp[i] = temp[i] / (height * width);
            }

            double result = 0;
            // 计算图像信息熵
            for (int i = 0; i < 256; i++)
            {
                if (temp[i] == 0.0)
                    result = result;
                else
                    result = result - temp[i] * (Math.Log(temp[i]) / Math.Log(2.0));
            }

            return result;

        }
        // 两幅图像联合信息熵计算
        double ComEntropy(byte[,] img1, byte[,] img2, double img1_entropy, double img2_entropy,int hei1,int hei2,int wid1,int wid2)
        {
            double[,] temp = new double[256, 256];

            // 计算联合图像像素的累积值
            for (int m1 = 0, m2 = 0; m1<hei1 && m2<hei2; m1++, m2++)
            {    // 有效访问行列的方式

                for (int n1 = 0, n2 = 0; n1 < wid1&&n2 < wid2; n1++, n2++)
                {
                    int i = img1[m1, n1], j = img2[m2, n2];
                    temp[i, j] = temp[i, j] + 1;
                }

            }

                // 计算每个联合像素的概率
            for (int i = 0; i< 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    temp[i, j] = temp[i, j] / (hei1 * wid1);
                }
            }

            double result = 0.0;
                    //计算图像联合信息熵
            for (int i = 0; i< 256; i++)
            {
                for (int j = 0; j< 256; j++)

                {
                    if (temp[i,j] == 0.0)
                        result = result;
                    else
                        result = result - temp[i,j] * (Math.Log(temp[i,j]) / Math.Log(2.0));
                }
            }

                    //得到两幅图像的互信息熵
            img1_entropy = Entropy(img1,hei1,wid1);
            img2_entropy = Entropy(img2,hei2,wid2);
            cd += img1_entropy;
            ef += img2_entropy;
            result = img1_entropy + img2_entropy - result;
            gh += result;
            return result; 
        }

        void FileOpen( TextBox sender) 
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK) 
            {
                sender.Text = openFile.FileName;
            }
        }
        /// <summary>
        /// 待匹配影像读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void open1_Click(object sender, EventArgs e)
        {
            FileOpen(textBox1);
            Bitmap curBitmap = new Bitmap(textBox1.Text);
            Color b = curBitmap.GetPixel(1, 0);
            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = curBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, curBitmap.PixelFormat);
            //得到首地址
            IntPtr ptr = bmpData.Scan0;
            //24位bmp位图字节数
            int bytes = bmpData.Stride * bmpData.Height;
            det_width = bmpData.Width;
            det_height = bmpData.Height;
            det_stride = bmpData.Stride;
            //把文件读取到字节数组
            byte[] det = new byte[bytes];                     
            System.Runtime.InteropServices.Marshal.Copy(ptr, det, 0, bytes);
            det_gray = new byte[det_height,det_width ];
            for (int i = 0; i < det_height; i++)
            {
                //利用公式（2.2）计算灰度值,只处理每行中是图像像素的数据，舍弃未用空间
                for (int j = 0; j < det_width * 3; j += 3)
                {
                    //colorTemp = rgbValues[i * bmpData.Stride + j + 2] * 0.299 + rgbValues[i * bmpData.Stride + j + 1] * 0.587 + rgbValues[i * bmpData.Stride + j] * 0.114;//之需改动此公式就可以改变灰度化图像
                    //           i * det_width + j / 3                                                                                                                                           //R=G=B
                    //rgbValues[i * bmpData.Stride + j + 2] = rgbValues[i * bmpData.Stride + j + 1] = rgbValues[i * bmpData.Stride + j] = (byte)colorTemp;
                    det_gray[i,j/3] = (byte)((det[i * bmpData.Stride + j] * 0.114 + det[i * bmpData.Stride + j + 1] * 0.587 + det[i * bmpData.Stride + j + 2] *0.299 ));
                    
                }
              

            }
            
            curBitmap.UnlockBits(bmpData);
            curBitmap.Save("1111.png");
            curBitmap.Dispose();
            bmpData = null;
            det = null;
            GC.Collect();
                     
        }
        /// <summary>
        /// 模板影像读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void open2_Click(object sender, EventArgs e)
        {
            FileOpen(textBox2);
            Bitmap curBitmap = new Bitmap(textBox2.Text);
            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = curBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, curBitmap.PixelFormat);
            //得到首地址
            IntPtr ptr = bmpData.Scan0;
            //24位bmp位图字节数
            int bytes = bmpData.Stride * bmpData.Height;
            src_width = bmpData.Width;
            src_height = bmpData.Height;
            src_stride = bmpData.Stride;
            //把文件读取到字节数组
            byte[] src = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, src, 0, bytes);
            src_gray = new byte[src_height, src_width];
            for (int i = 0; i < src_height; i++)
            {
                //利用公式（2.2）计算灰度值,只处理每行中是图像像素的数据，舍弃未用空间
                for (int j = 0; j < src_width * 3; j += 3)
                {
                    //colorTemp = rgbValues[i * bmpData.Stride + j + 2] * 0.299 + rgbValues[i * bmpData.Stride + j + 1] * 0.587 + rgbValues[i * bmpData.Stride + j] * 0.114;//之需改动此公式就可以改变灰度化图像
                    //                      i * src_width + j / 3                                                                                                                                //R=G=B
                    //rgbValues[i * bmpData.Stride + j + 2] = rgbValues[i * bmpData.Stride + j + 1] = rgbValues[i * bmpData.Stride + j] = (byte)colorTemp;
                    src_gray[i,j/3] = (byte)(src[i * bmpData.Stride + j] *0.114  + src[i * bmpData.Stride + j + 1] * 0.587 + src[i * bmpData.Stride + j + 2] *0.299 );
                }
                

            }
            curBitmap.UnlockBits(bmpData);
            curBitmap.Dispose();
            bmpData = null;
            src = null;
            GC.Collect();
            form2 = new Form1();           
        }
        /// <summary>
        /// 数据格式，第一行为名字，第二行开始为数据
        /// </summary>
        /// <param name="path"></param>
        public void txt2dataTable(string path, int n)
        {
            StreamReader stream = new StreamReader(path, System.Text.Encoding.Default);
            string name = stream.ReadLine();
            string[] readerofColumnNumber = name.Split(',');
            foreach (var i in readerofColumnNumber)
            {
                DataColumn col = Data.Tables[n].Columns.Add(i.ToString());
            }
            string nextLine;
            while ((nextLine = stream.ReadLine()) != null)
            {
                string[] every_row = nextLine.Split(',');
                DataRow dr = Data.Tables[n].NewRow();
                count = Data.Tables[n].Columns.Count;
                for (int i = 0; i < count; i++)
                {
                    dr[i] = every_row[i];
                }
                Data.Tables[n].Rows.Add(dr);
            }

        }

        private void 选取待匹配图像及模板图像_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileOpen(textBox3);
            DataTable table = new DataTable();
            Data.Tables.Add(table);
            txt2dataTable(textBox3.Text, Data.Tables.Count - 1);
            label3.Text = Data.Tables[Data.Tables.Count - 1].Rows.Count.ToString() + "个点";

        }

        public struct point 
        {
            public point(int x, int y) 
            {
                maxi = x; maxj = y;
            }
            public int maxi, maxj;
        }
        List<point> points = new List<point>();
        int winRow, winCol;

       
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            open1_Click(sender, e);
            double c=Entropy(det_gray, det_height, det_width);
            textBox9.Text = Convert.ToString(c);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            open1_Click(sender, e);
            open2_Click(sender, e);
            double c = ComEntropy(det_gray, src_gray, Entropy(det_gray, det_height, det_width), Entropy(src_gray, src_height, src_width), det_height, src_height, det_width, src_width);
            textBox10.Text = Convert.ToString(c);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        int realRow, realCol;
        int maxi, maxj;//最大值及所处位置
        
        byte[] chances;
      

        public point NCC(int numOfTable) 
        {

            
            int num = Convert.ToInt32(textBox4.Text);//匹配窗口大小
            byte[] window = new byte[num * num];//匹配窗口
            //模板所处位置
            winCol = Convert.ToInt32(Data.Tables[0].Rows[numOfTable][1]);
            winRow = Convert.ToInt32(Data.Tables[0].Rows[numOfTable][2]);
            //参照真值位置
            realRow = Convert.ToInt32(Data.Tables[0].Rows[numOfTable][4]);
            realCol = Convert.ToInt32(Data.Tables[0].Rows[numOfTable][3]);
            
            double _srcgray = 0;

            //模板影像
            for (int i = winCol; i < winCol + num; i++)
            {
                for (int j = winRow; j < winRow + num; j++)
                {
                    int row = j - winRow;
                    int col = i - winCol;
                    window[col * num + row] = src_gray[i , j];
                    _srcgray += src_gray[i , j];
                }
            }
            _srcgray /= (num * num * 1.0);//模板平均值                  
            ///NCC值（明天再写，对chance赋值，处理det_gray和window）
            double maxchance = 0.0;
            for (int i = 0; i < det_height - num; i++)
            {
                for (int j = 0; j < det_width - num; j++)
                {
                    double _detgray = 0.0;
                    for (int k = i; k < i + num; k++)
                    {
                        for (int l = j; l < j + num; l++)
                        {
                            _detgray += det_gray[k , l];
                        }
                    }
                    _detgray /= (num * num * 1.0);
                    double UpSum = 0.0, DownSumSrc2 = 0.0, DownSumDet2 = 0.0;
                    for (int k = i; k < i + num; k++)
                    {
                        for (int l = j; l < j + num; l++)
                        {
                            UpSum += (window[(k - i) * num + (l - j)] - _srcgray) * (det_gray[k ,l] - _detgray);/** det_width*/
                            DownSumSrc2 += (window[(k - i) * num + (l - j)] - _srcgray) * (window[(k - i) * num + (l - j)] - _srcgray);
                            DownSumDet2 += (det_gray[k , l] - _detgray) * (det_gray[k  ,l] - _detgray);/** det_width +*/


                        }
                    }


                    double curchance = UpSum / Math.Sqrt(DownSumDet2 * DownSumSrc2);
                    if (maxchance < curchance) 
                    {
                        maxchance = curchance;
                        maxi = i;
                        maxj = j;
                    }
                }
            }
            point _points;
            _points.maxi = maxi;
            _points.maxj = maxj;
            return _points;
        }
       
        public point NCC(Bitmap bmp,int x,int y)
        {

            
            int num = Convert.ToInt32(textBox4.Text);//匹配窗口大小
            
            byte[] window = new byte[num * num];//匹配窗口
            //模板所处位置
            winCol = y;
            winRow = x;
            //参照真值位置
           
            double _srcgray = 0;

            //模板影像
            for (int i = winCol; i < winCol + num; i++)
            {
                for (int j = winRow; j < winRow + num; j++)
                {
                    int row = j - winRow;
                    int col = i - winCol;
                    window[col * num + row] = src_gray[i, j];/** src_stride +*/
                    _srcgray += src_gray[i, j];/** src_stride +*/
                    

                }
            }
            byte[,] _window = new byte[num, num];
            byte[,] _window1 = new byte[num, num];
            for (int i = 0; i < num; i++) 
            {
                for(int j = 0; j < num; j++) 
                {
                    _window[i, j] = window[i * num + j];
                }
            }
            _srcgray /= (num * num * 1.0);//模板平均值    

            ///NCC值（明天再写，对chance赋值，处理det_gray和window）
            double maxchance = 0.0, temp = 0, temp1 = 0;
            for (int i = winCol; i < winCol+1; i++)
            {
                double t = winRow - Convert.ToDouble(textBox7.Text) > 0 ? winRow - Convert.ToDouble(textBox7.Text) : 0;
                double m = t + Convert.ToDouble(textBox7.Text) > bmp.Width ? bmp.Width : t + Convert.ToDouble(textBox7.Text) ;
                for (int j = (int)t; j < m; j++)
                {
                    double _detgray = 0.0;
                    for (int k = i; k < i + num; k++)
                    {
                        for (int l = j; l < j + num; l++)
                        {
                            _detgray += det_gray[k , l];/** det_stride +*/
                        }
                    }
                    _detgray /= (num * num * 1.0);
                    double UpSum = 0.0, DownSumSrc2 = 0.0, DownSumDet2 = 0.0;
                    for (int k = i; k < i + num; k++)
                    {
                        for (int l = j; l < j + num; l++)
                        {
                            UpSum += (window[(k - i) * num + (l - j)] - _srcgray) * (det_gray[k, l] - _detgray);/* * det_stride +*/
                            DownSumSrc2 += (window[(k - i) * num + (l - j)] - _srcgray) * (window[(k - i) * num + (l - j)] - _srcgray);
                            DownSumDet2 += (det_gray[k ,l] - _detgray) * (det_gray[k  ,l] - _detgray);/** det_stride +*/


                        }
                    }


                    double curchance = UpSum / Math.Sqrt(DownSumDet2 * DownSumSrc2);
                    if (maxchance < curchance)
                    {
                        maxchance = curchance;
                        maxi = i;
                        maxj = j;
                    
                    }

                }
            }
            point _points;
            _points.maxi = maxi;
            _points.maxj = maxj;
            for (int k = maxi; k < maxi + num; k++)
            {
                for (int l = maxj; l < maxj + num; l++)
                {
                    
                    _window1[(k - maxi), (l - maxj)] = det_gray[k, l];
                }
            }
            ComEntropy(_window, _window1, cd, ef, num, num, num, num);
            return _points;
        }
        private void load_Click(object sender, EventArgs e)
        {
            if (Data.Tables.Count == 0)
            {
                if (Mode == "single")
                {

                    Bitmap bitmap1 = new Bitmap(textBox1.Text);
                    BitmapData bmpData1 = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    int stride1 = bmpData1.Stride;
                    int offset1 = stride1 - det_width;
                    IntPtr iptr1 = bmpData1.Scan0;
                    int scanBytes1 = stride1 * det_height;
                    byte[] pixelValues1 = new byte[scanBytes1];
                    Bitmap bitmap2 = new Bitmap(textBox2.Text);
                    BitmapData bmpData2 = bitmap2.LockBits(new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    int stride2 = bmpData2.Stride;
                    int offset2 = stride2 - det_width;
                    IntPtr iptr2 = bmpData2.Scan0;
                    int scanBytes2 = stride2 * det_height;
                    byte[] pixelValues2 = new byte[scanBytes2];
                    int num = Convert.ToInt32(textBox4.Text);
                    //复制被锁定的位图像值到该数组内
                    System.Runtime.InteropServices.Marshal.Copy(iptr1, pixelValues1, 0, scanBytes1);
                    System.Runtime.InteropServices.Marshal.Copy(iptr2, pixelValues2, 0, scanBytes1);
                    for (int i = 0; i < det_height; i++)
                    {

                        for (int j = 0; j < det_width; j++)
                        {


                            pixelValues1[i * stride1 + j] = (byte)(4 * Math.Abs(pixelValues1[i * stride1 + j] - pixelValues2[i * stride2 + j]));


                        }

                    }
                    Bitmap bmp = new Bitmap(det_width, det_height, PixelFormat.Format8bppIndexed);
                    BitmapData bmp1 = bmp.LockBits(new Rectangle(0, 0, det_width, det_height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    IntPtr intPtr3 = bmp1.Scan0;
                    System.Runtime.InteropServices.Marshal.Copy(pixelValues1, 0, intPtr3, scanBytes1);
                    bmp.UnlockBits(bmp1);
                    bitmap1.UnlockBits(bmpData1);
                    ColorPalette tempPalette;
                    using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                    {
                        tempPalette = tempBmp.Palette;
                    }
                    for (int i = 0; i < 256; i++)
                    {
                        tempPalette.Entries[i] = Color.FromArgb(i, i, i);
                    }

                    bmp.Palette = tempPalette;
                    bmp.Save("4倍视差图");
                }
                else
                {
                    int num = Convert.ToInt32(textBox4.Text);
                    Bitmap bmp = new Bitmap(det_width-num , det_height - num,PixelFormat.Format24bppRgb);

                    //BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, det_width - num, det_height - num),
                    // ImageLockMode.WriteOnly,PixelFormat.Format24bppRgb);
                    //int stride = bmpData.Stride;
                    //int offset = stride - det_width;
                    //IntPtr iptr = bmpData.Scan0;
                    //int scanBytes = (det_width - num) * 3 * (det_height - num);
                    byte[,] pixelValues = new byte[bmp.Height, bmp.Width];
                    //byte[] pp = new byte[(bmp.Height ) * (det_width - num)*3];
                    
                    for (int i = 0; i < det_height - num; i++)
                    {

                        for (int j = 0; j < (det_width - num); j++)
                        {


                            point p = NCC(bmp, j, i);

                            pixelValues[i, j] = (byte)(Math.Abs(p.maxj - j) * Convert.ToInt32(textBox5.Text));

                        }

                    }
                    cd /= (det_width - num) * 1.0 * (det_height - num) * 1.0;
                    ef /= (det_width - num)*1.0 * (det_height - num) * 1.0;
                    gh /= (det_width - num) * 1.0 * (det_height - num) * 1.0;
                    textBox9.Text = Convert.ToString(cd);
                    textBox10.Text = Convert.ToString(ef);
                    textBox13.Text = Convert.ToString(gh);
                    cd = 0;
                    ef = 0;
                    gh = 0;
                    for (int i = 0; i < det_height - num; i++)
                    {

                        for (int j = 0; j < (det_width - num); j++)
                        {
                            bmp.SetPixel(j, i, Color.FromArgb(pixelValues[i, j], pixelValues[i, j], pixelValues[i, j]));

                        }
                    }
                    SaveFileDialog saveFile = new SaveFileDialog();
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        bmp.Save(saveFile.FileName, ImageFormat.Png);
                    }

                    bmp = null;
                }


            }
            else 
            {
                int[] pop = new int[256];
                int num = Convert.ToInt32(textBox4.Text);
                int cou = 0;
                Bitmap bmp = new Bitmap(det_width - num, det_height - num, PixelFormat.Format8bppIndexed);
                for (int i = 0; i < Data.Tables[0].Rows.Count; i++) 
                {
                    int ii = Convert.ToInt32(Data.Tables[0].Rows[i][1]);
                    int jj = Convert.ToInt32(Data.Tables[0].Rows[i][2]);
                    point point = NCC(bmp, Convert.ToInt32(Data.Tables[0].Rows[i][1]), Convert.ToInt32(Data.Tables[0].Rows[i][2]));
                    
                    cou = Math.Abs(point.maxj - Convert.ToInt32(Data.Tables[0].Rows[i][3]));
                    pop[Math.Abs(point.maxj - Convert.ToInt32(Data.Tables[0].Rows[i][3]))]++;
                        
                    
                    
                    
                    
                }
                double[] chancee = new double[Data.Tables[0].Rows.Count];
                for (int i = 0; i < Data.Tables[0].Rows.Count; i++)
                {
                    chancee[i] = pop[i] * 1.0 / Data.Tables[0].Rows.Count;


                }
                textBox12.Text = Convert.ToString(chancee[0]);
            }
            


        }

       

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
