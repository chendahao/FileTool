using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenameTOOL
{
    public class Tool
    {
        public string SelectedPath()
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            var p = path.SelectedPath.ToString();
            return p;
        }
    }
}
