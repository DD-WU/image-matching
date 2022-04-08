using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 毕业设计
{
    class 多窗口:ApplicationContext
    {
        private void onFormClose(object sendr, EventArgs e)
        {

            if (Application.OpenForms.Count == 0)
            {
                ExitThread();
            }
        }
        public 多窗口()
        {
           
            
            toolsbar form2 = new toolsbar();
            var formlist = new List<Form>() { form2 };

            foreach (var item in formlist)
            {
                item.Show();

            }
        }


    }
}
