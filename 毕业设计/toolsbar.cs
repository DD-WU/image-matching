using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 毕业设计
{
    public partial class toolsbar : Form
    {

        图片列表 form = new 图片列表();
        public toolsbar()
        {
            InitializeComponent();
        }

        private void openImageFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form.button1_Click_1(sender,e);
        }

        private void 图片管理器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form.Visible = true;
        }

        private void toolsbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }

        private void singleToolStripMenuItem_Click(object sender, EventArgs e)
        {

            选取待匹配图像及模板图像 a = new 选取待匹配图像及模板图像("single");
            a.button1.Visible = true;
            a.textBox3.Visible = true;
            a.label2.Visible = true;
            a.Show();
        }

        private void multiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            选取待匹配图像及模板图像 a = new 选取待匹配图像及模板图像("multi");
            a.Show();
        }

        private void minusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            做差 zuocha = new 做差();
            zuocha.Show();
        }
    }
}
