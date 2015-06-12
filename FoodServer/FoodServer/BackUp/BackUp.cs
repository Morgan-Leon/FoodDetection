using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS;

namespace FoodServer.BackUp
{
    public partial class BackUp : Form
    {
        public BackUp()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBackup_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.InitialDirectory = "c:";//注意这里写路径时要用c:而不是c:　
            saveFileDlg.Filter = "所有文件|*.*";
            saveFileDlg.RestoreDirectory = true;

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                IniAc iniFile = new IniAc();
                string user = iniFile.ReadValue("DBMS", "user");
                string pwd = iniFile.ReadValue("DBMS", "pwd");
                string database = iniFile.ReadValue("DBMS", "database");

                string fileName = saveFileDlg.FileName;
                string cmdStr = "/c mysqldump " + " -u " + user +
                    " -p" + pwd + " " + database + " " + " > " + fileName;

                try
                {
                    System.Diagnostics.Process.Start("cmd", cmdStr);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// 恢复数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRestore_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:";//注意这里写路径时要用c:而不是c:　
            openFileDialog.Filter = "所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                IniAc iniFile = new IniAc();
                string user = iniFile.ReadValue("DBMS", "user");
                string pwd = iniFile.ReadValue("DBMS", "pwd");
                string database = iniFile.ReadValue("DBMS", "database");

                string cmdStr = "/c mysql " + " --user=" + user +
                    " --password=" + pwd + " " + "bbcc" + " " + " < " + fileName;

                try
                {
                    System.Diagnostics.Process.Start("cmd", cmdStr);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void buttonBackupSettings_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.InitialDirectory = "c:";//注意这里写路径时要用c:而不是c:　
            saveFileDlg.Filter = "所有文件|*.*";
            saveFileDlg.RestoreDirectory = true;

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                IniAc iniFile = new IniAc();
                string user = iniFile.ReadValue("DBMS", "user");
                string pwd = iniFile.ReadValue("DBMS", "pwd");
                string database = iniFile.ReadValue("DBMS", "database");
                string tables = iniFile.ReadValue("BACKUP", "tables");

                string fileName = saveFileDlg.FileName;
                string cmdStr = "/c mysqldump " + " -u " + user +
                    " -p" + pwd + " " + database + " " + tables + " " +
                    " > " + fileName;

                try
                {
                    System.Diagnostics.Process.Start("cmd", cmdStr);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void buttonRestoreSettings_Click(object sender, EventArgs e)
        {

        }
    }
}
