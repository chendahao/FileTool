using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace RenameTOOL
{
    public partial class Form1 : Form
    {
        public string fileNamePublic = "";//文件名
        public string filePathPublic = "";//文件路径
        public int time = 0;
        int currentCol = -1;
        bool sort;
        Tool tool=new Tool();
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 在资源管理器中选择文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            try
            {
                var p = path.SelectedPath.ToString();
                DirectoryInfo di = new DirectoryInfo(p);
                if (di.GetFiles().Count() > 0)
                {
                    for (int i = 0; i < di.GetFiles().Count(); i++)
                    {
                        FileInfo fi = di.GetFiles()[i];
                        var filename = di.GetFiles()[i].Name;
                        var newname = "new" + filename;
                        //var t= di.GetFiles()[i].Extension;   //后缀名
                        //var pathin = di.GetFiles()[i].DirectoryName;    //路径
                        //var d = di.GetFiles()[i].FullName;    //完整路径
                        fi.MoveTo(Path.Combine(fi.DirectoryName, newname));
                    }
                }
                else
                {
                    MessageBox.Show("你选择的文件夹为空");
                }
            }
            catch
            {
                this.Close();
            }
        }

        private class IconIndexes
        {
            public const int MyComputer = 0;
            public const int ClosedFolder = 1;
            public const int OpenFolder = 2;
            public const int FixedDrive = 3;
            public const int MyDocuments = 4;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //实例化treenode类，
            TreeNode rootNode = new TreeNode("我的电脑",IconIndexes.MyComputer, IconIndexes.MyComputer);
            //树节点数据
            rootNode.Tag = "我的电脑";
            //树节点标签内容
            rootNode.Text = "我的电脑";
            //树中添加根目录
            this.directoryTree.Nodes.Add(rootNode);

            //显示我的文档节点
            //var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //TreeNode DocNode = new TreeNode(myDocuments);
            //DocNode.Tag = "我的文档";
            //DocNode.Text = "我的文档";
            //DocNode.ImageIndex = IconIndexes.MyDocuments;           //设置获取节点显示图片
            //DocNode.SelectedImageIndex = IconIndexes.MyDocuments;   //设置选择显示图片
            //rootNode.Nodes.Add(DocNode);                            //rootnode 目录下加载节点
            //DocNode.Nodes.Add("");

            //循环遍历计算机所有盘符
            //var a = Environment.Is64BitOperatingSystem;
            //var b = Environment.MachineName;
            //var ee = Environment.CommandLine;
            //var d = Environment.ProcessorCount;
            //var c = Environment.OSVersion;
            foreach (string drive in Environment.GetLogicalDrives())
            {
                //实例化dirveinfo对象  
                var dir = new DriveInfo(drive);
                switch (dir.DriveType)             //判断驱动器类型
                {
                    case DriveType.Fixed:          //仅取固定磁盘符  
                        {
                            //获取判读字母
                            TreeNode tNode = new TreeNode(dir.Name.Split(':')[0]+"盘");
                            tNode.Name = dir.Name;
                            tNode.Tag = tNode.Name;
                            tNode.ImageIndex = IconIndexes.FixedDrive;
                            tNode.SelectedImageIndex = IconIndexes.FixedDrive;
                            directoryTree.Nodes.Add(tNode);
                            tNode.Nodes.Add("");   //加载空节点 实现+号
                        }
                        break;
                        //U盘 
                    case DriveType.Removable:          
                        {
                            //获取判读字母
                            TreeNode tNode = new TreeNode(dir.Name.Split(':')[0] + "盘");
                            tNode.Name = dir.Name;
                            tNode.Tag = tNode.Name;
                            tNode.ImageIndex = IconIndexes.FixedDrive;
                            tNode.SelectedImageIndex = IconIndexes.FixedDrive;
                            directoryTree.Nodes.Add(tNode);
                            tNode.Nodes.Add("");   //加载空节点 实现+号
                        }
                        break;
                }
            }
            rootNode.Expand();
            SetListView();
        }

        private void directoryTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.Expand();
        }

        private void directoryTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeViewItems.Add(e.Node);
        }
        public static class TreeViewItems
        {
            public static void Add(TreeNode e)
            {
                try
                {
                    if (e.Tag.ToString()!="我的电脑")
	                {
		                e.Nodes.Clear();
                        TreeNode tNode=e;
                        string path=tNode.Name;

                        //获取“我的文档”路径
                        if (e.Tag.ToString()=="我的文档")
	                    {
                            path=Environment.GetFolderPath
                                (Environment.SpecialFolder.MyDocuments);
	                    }
                        //获取指定目录中的子目录并加载节点
                        string[] dics=Directory.GetDirectories(path);
                        foreach (var dic in dics)
	                    {
                            DirectoryInfo di = new DirectoryInfo(dic);
		                    TreeNode subNode=new TreeNode(di.Name);
                            subNode.Name=new DirectoryInfo(dic).FullName;
                            //处理当点击此文件夹时报错的情况
                            if (subNode.Name.Contains("System Volume Information"))
                            {
                                continue;
                            }
                            //跳过隐藏文件
                            if (di.Attributes.ToString().Contains("Hidden"))
	                        {
                                continue;
	                        }

                            subNode.Tag=subNode.Name;
                            subNode.ImageIndex=IconIndexes.ClosedFolder;
                            subNode.SelectedImageIndex=IconIndexes.OpenFolder;
                            tNode.Nodes.Add(subNode);
                            subNode.Nodes.Add("");
	                    }
	                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void SetListView() 
        {
            this.filesList.GridLines = true;  //行和列是否显示网格线
            this.filesList.View = View.Details;  //显示方式
            this.filesList.LabelEdit = true;  //是否可以编辑
            this.filesList.HeaderStyle = ColumnHeaderStyle.Clickable; //表头进行设置
            this.filesList.FullRowSelect = true;//是否可以选择行
            this.filesList.Scrollable = true;
            //设置listview 列标题头， 宽度为9/13  2/13 2/13 
            //设置标题头自动适应宽度，-1 根据内容设置宽度，-2根据标题设置宽度
            this.filesList.Columns.Add("序号", 2 * filesList.Width / 19); 
            this.filesList.Columns.Add("名称", 8 * filesList.Width / 19);
            this.filesList.Columns.Add("大小", 3 * filesList.Width / 19);
            this.filesList.Columns.Add("类型", 2 * filesList.Width / 19);
            this.filesList.Columns.Add("创建时间", 4 * filesList.Width / 19);
        }
        /// <summary>
        /// 获取节点的路径：递归调用产生节点对文件夹的路径
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetPathFromNode(TreeNode node)
        {
            if (node.Parent==null)
            {
                return node.Name;
            }
            //path.Combine 组合的产生的路径   path.combine（"A"，"B"） 则生成“A\\B”
            return Path.Combine(GetPathFromNode(node.Parent),node.Name);
        }
        private void directoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                long length; //文件大小
                string path; //文件路径
                TreeNode clickedNode = e.Node;  //当前选中的节点

                //移除listview所有项
                this.filesList.Items.Clear();

                //获取路径赋值path
                if (clickedNode.Tag.ToString()=="我的文档")
                {
                    //获取计算机我的文档文件夹
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else
                {
                    //通过自定义函数getpathfromnode获取节点路径
                    path = GetPathFromNode(clickedNode);
                }
                //由于“我的电脑”为空节点，无需处理，否组会出现路径获取错误或没有找到‘我的电脑’路径
                if (clickedNode.Tag.ToString() != "我的电脑")
                {
                    //数据更新 UI 暂时挂起直到endupdate 绘制控件，可以有效避免闪烁并大大提高加载速度
                    this.filesList.BeginUpdate();
                    //实例目录与子目录
                    DirectoryInfo dir = new DirectoryInfo(path);
                    filePathPublic = path;
                    //获取当前目录文件列表
                    FileInfo[] fileInfo = dir.GetFiles();
                    //循环输出获取文件信息
                    for (int i = 0; i < fileInfo.Length; i++)
                    {
                        ListViewItem listItem = new ListViewItem();
                        listItem.Text = (i + 1).ToString();  //序号
                        //listItem.ForeColor = Color.Red;
                        length = fileInfo[i].Length;//文件大小
                        listItem.SubItems.Add(fileInfo[i].Name);
                        decimal size=Math.Ceiling(decimal.Divide(length,1024));
                        if (size>1024)
                        {
                            size = Math.Ceiling(decimal.Divide(size, 1024));
                            listItem.SubItems.Add(size + "MB");
                        }
                        else
                        {
                            listItem.SubItems.Add(size+"KB");
                        }
                        listItem.SubItems.Add(fileInfo[i].Extension);
                        listItem.SubItems.Add(fileInfo[i].CreationTime.ToString());
                        //加载数据至fileslist
                        this.filesList.Items.Add(listItem);
                    }
                    for (int i = 0; i < filesList.Items.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            filesList.Items[i].BackColor = Color.AliceBlue;
                            //for (int j = 0; j < filesList.Items[i].SubItems.Count; j++)
                            //{
                            //    filesList.Items[i].SubItems[j].BackColor = Color.AliceBlue;
                            //}
                        }
                    }
                    //结束数据处理，ui界面一次性绘制，否自可能出现闪动情况
                    this.filesList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void filesList_MouseClick(object sender, MouseEventArgs e)
        {
            //禁止多选
            filesList.MultiSelect = false;
            //鼠标右键
            if (e.Button==MouseButtons.Right)
            {
                string fileName = filesList.SelectedItems[0].Text;//选中的文件名
                Point p = new Point(e.X,e.Y);
                contextMenuStrip1.Show(filesList,p);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.filesList.SelectedItems.Count==0)
                return;
            var selecteditem=this.filesList.SelectedItems[0];
            //全局变量文件名  subitem【1】 表示文件名
            fileNamePublic = filePathPublic + "\\" + selecteditem.SubItems[1].Text;
            try
            {
                //实例化一个新的process类， 
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = fileNamePublic;//指定要启动的文件路径
                    p.StartInfo.CreateNoWindow = false;//在当前窗口启动程序
                    p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private const int FO_DELETE = 3;                    //删除
        private const int FOF_SILENT = 0x0004;          //不显示进度条提示框
        private const int FOF_NOCONFIRMATTON = 0x0010;  //不出现任何对话框
        private const int FOF_ALLOWUNDO = 0x0040;//允许撤销
        private const int FOF_NOCONFIRMMKDIR = 0x0200;//创建文件夹的时候不用确认

        [StructLayout(LayoutKind.Sequential)]
        private struct ShFileOpstruct
        {
            public int hwnd; //父窗口句柄，0为桌面
            public int wFunc;//功能标志 F0_copy 复制
            public string pFrom;//source file 源文件或者源文件夹
            public string pTo;//destination 目的文件或文件夹 
            public int fFlags;//控制文件的标志位
            public bool fAnyOperationsAborted;
            public int hNameMappings;
            public string lpszProgressTitle;
        }
        [DllImport("shell32.dll")]
        private static extern int SHFileOperation(ref ShFileOpstruct FileOp);
        private static int Delete(string sPath, bool recycle)
        {
            ShFileOpstruct FileOp = new ShFileOpstruct();
            FileOp.hwnd = 0;
            FileOp.wFunc = FO_DELETE;
            FileOp.fFlags = 0;
            FileOp.fFlags = FileOp.fFlags | FOF_SILENT;
            FileOp.fFlags = FileOp.fFlags | FOF_NOCONFIRMATTON;
            FileOp.fFlags = FileOp.fFlags | FOF_NOCONFIRMMKDIR;
            if (recycle)
            {
                FileOp.fFlags = FileOp.fFlags | FOF_ALLOWUNDO;
            }
            FileOp.pFrom = sPath + "\0";
            return SHFileOperation(ref FileOp);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.filesList.SelectedItems.Count == 0) return;
            var selecteditem = this.filesList.SelectedItems[0];
            fileNamePublic = filePathPublic + "\\" + selecteditem.SubItems[1].Text;
            try
            {
                if (MessageBox.Show("确定删除？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //删除文件
                    Delete(fileNamePublic, true);
                    foreach (ListViewItem item in this.filesList.SelectedItems)
                    {
                        this.filesList.Items.Remove(item);
                    }
                }
                MessageBox.Show(this, "成功删除", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            var formwidth=Form1.ActiveForm.Width;
            this.directoryTree.Width = formwidth / 5;
        }

        private void filesList_DoubleClick(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, EventArgs.Empty);
        }

        private void fileopenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.filesList.SelectedItems.Count == 0)
                return;
            var selecteditem = this.filesList.SelectedItems[0];
            //全局变量文件名  subitem【1】 表示文件名
            fileNamePublic = filePathPublic + "\\" + selecteditem.SubItems[1].Text;
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileNamePublic;
            System.Diagnostics.Process.Start(psi);
        }

        private void filesList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //在选中的列上加 升序和降序的三角
            string ASC = ((char)0x25bc).ToString().PadLeft(1, ' ');    
            string DES = ((char)0x25b2).ToString().PadLeft(1, ' ');
            if (sort==false)
            {
                sort = true;
                string oldstr = this.filesList.Columns[e.Column].Text.TrimEnd((char)0x25bc, (char)0x25b2, ' ');
                this.filesList.Columns[e.Column].Text = oldstr + DES;
            }
            else if (sort == true)
            {
                sort = false;
                string oldstr = this.filesList.Columns[e.Column].Text.TrimEnd((char)0x25bc, (char)0x25b2, ' ');
                this.filesList.Columns[e.Column].Text = oldstr + ASC;
            }


            if (filesList.Columns[e.Column].Text.Contains("序号"))
            {           
                filesList.ListViewItemSorter = new ListViewItemComparerNum(e.Column,sort);
            }
            else if (filesList.Columns[e.Column].Text.Contains("大小"))
            {
                filesList.ListViewItemSorter = new ListViewItemComparerSize(e.Column,sort);
            }
            else
            {
                filesList.ListViewItemSorter = new ListViewItemComparer(e.Column, sort);
            }
            this.filesList.Sort();
            int rowCount = this.filesList.Items.Count;
            if (currentCol!=-1)
            {
                if (e.Column!=currentCol)
                {
                    this.filesList.Columns[currentCol].Text = this.filesList.Columns[currentCol].Text.TrimEnd((char)0x25bc, (char)0x25b2, ' ');
                }
            }
            currentCol = e.Column;
        }

        public class ListViewItemComparer : IComparer
        {
            public bool sort_b;
            public SortOrder order = SortOrder.Ascending;
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column, bool sort)
            {
                col = column;
                sort_b = sort;
            }
            public int Compare(object x, object y)
            {
                if (sort_b)
                {
                    return string.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                }
                else
                {
                    return string.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
                }
            }
        }
        public class ListViewItemComparerNum : IComparer
        {
            public bool sort_b;
            public SortOrder order = SortOrder.Ascending;
            private int col;
            public ListViewItemComparerNum()
            {
                col = 0;
            }
            public ListViewItemComparerNum(int column, bool sort)
            {
                col = column;
                sort_b = sort;
            }
            public int Compare(object x, object y)
            {
                decimal d1 = Convert.ToDecimal(((ListViewItem)x).SubItems[col].Text);  //.Replace("["," ").Replace("]"," ").Trim()
                decimal d2 = Convert.ToDecimal(((ListViewItem)y).SubItems[col].Text);
                if (sort_b)
                {
                    return decimal.Compare(d1, d2);
                }
                else
                {
                    return decimal.Compare(d2, d1);
                }
            }
        }
        public class ListViewItemComparerSize : IComparer
        {
            public bool sort_b;
            public SortOrder order = SortOrder.Ascending;
            private int col;
            public ListViewItemComparerSize()
            {
                col = 0;
            }
            public ListViewItemComparerSize(int column, bool sort)
            {
                col = column;
                sort_b = sort;
            }
            public int Compare(object x, object y)
            {
                var x1 = ((ListViewItem)x).SubItems[col].Text;
                int x2=0,y2=0;
                if (x1!=null)
                {
                    if (x1.Contains("MB"))
                    {
                        x1=x1.Substring(0,x1.Length-2);
                        x2 = Convert.ToInt32(x1) * 1024;
                    }
                    else
                    {
                        x1 = x1.Substring(0, x1.Length - 2);
                        x2 = Convert.ToInt32(x1);
                    }
                }
                var y1 = ((ListViewItem)y).SubItems[col].Text;
                if (y1!=null)
                {
                    if (y1.Contains("MB"))
                    {
                        y1 = y1.Substring(0, y1.Length - 2);
                        y2 = Convert.ToInt32(y1) * 1024;
                    }
                    else
                    {
                         y1=y1.Substring(0,y1.Length-2);
                         y2 = Convert.ToInt32(y1);
                    }
                }
                decimal d1 = Convert.ToDecimal(x2);  
                decimal d2 = Convert.ToDecimal(y2);
                if (sort_b)
                {
                    return decimal.Compare(d1, d2);
                }
                else
                {
                    return decimal.Compare(d2, d1);
                }
            }
        }
        //private void filesList_ColumnClick(object sender, ColumnClickEventArgs e)
        //{
        //    //获取选中列的名称
        //    //var a = filesList.Columns[e.Column];
        //    //time += 1;
        //    //if (a.Text=="名称")
        //    //{
        //    //    if (time%2==1)
        //    //    {
        //    //        filesList.Sorting = SortOrder.None;
        //    //        //filesList.Sorting = SortOrder.Ascending;
        //    //    }
        //    //    else
        //    //    {
        //    //        filesList.Sorting = SortOrder.Ascending;
        //    //        //filesList.Sorting = SortOrder.Descending;
        //    //    }
        //    //}
        //    //else if (a.Text=="类型")
        //    //{
        //    //    if (time % 2 == 1)
        //    //    {
        //    //        filesList.Sorting = SortOrder.None;
        //    //        //filesList.Sorting = SortOrder.Ascending;
        //    //    }
        //    //    else
        //    //    {
        //    //        filesList.Sorting = SortOrder.Ascending;
        //    //        //filesList.Sorting = SortOrder.Descending;
        //    //    }
        //    //}
        //}

    }
}
