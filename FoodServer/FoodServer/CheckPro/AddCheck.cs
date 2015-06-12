using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FoodServer.checkedUnit;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;
using FoodServer.Class;

namespace FoodServer.CheckPro
{
    public partial class AddCheck : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();
        CheckedUnit.AlarmParameter parameter = new CheckedUnit.AlarmParameter();
        int IDNum = 0;
        public AddCheck()
        {
            InitializeComponent();
           
            databaseName = dbMySql.GetDatabaseName();
        }
        ChekPro parent;
        public AddCheck(CheckedUnit.AlarmParameter par, ChekPro cu)
        {
            InitializeComponent();
            
            databaseName = dbMySql.GetDatabaseName();
            parameter = par;
            parent = cu;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strsql;
            FINFOR_INFOSTR ftinfostr = new FINFOR_INFOSTR();
            int j = parent.dataGridView1.CurrentCell.RowIndex;

            ftinfostr.ftesttimes = parent.dataGridView1.Rows[j].Cells[1].Value.ToString();
            ftinfostr.fsample = parent.dataGridView1.Rows[j].Cells[2].Value.ToString();
            ftinfostr.wavemode = parent.dataGridView1.Rows[j].Cells[3].Value.ToString();
            ftinfostr.wavemajor = parent.dataGridView1.Rows[j].Cells[4].Value.ToString();
            ftinfostr.waveminor = parent.dataGridView1.Rows[j].Cells[5].Value.ToString();
            ftinfostr.formulaC = parent.dataGridView1.Rows[j].Cells[6].Value.ToString();
            ftinfostr.formulaB = parent.dataGridView1.Rows[j].Cells[7].Value.ToString();
            ftinfostr.formulaA = parent.dataGridView1.Rows[j].Cells[8].Value.ToString();
            ftinfostr.interdilute = parent.dataGridView1.Rows[j].Cells[9].Value.ToString();
            ftinfostr.unit = parent.dataGridView1.Rows[j].Cells[10].Value.ToString();
            ftinfostr.standards = parent.dataGridView1.Rows[j].Cells[11].Value.ToString();
            ftinfostr.comparemode = parent.dataGridView1.Rows[j].Cells[12].Value.ToString();
            ftinfostr.comparemin = parent.dataGridView1.Rows[j].Cells[13].Value.ToString();
            ftinfostr.comparemax = parent.dataGridView1.Rows[j].Cells[14].Value.ToString();
            ftinfostr.tesrangemax = parent.dataGridView1.Rows[j].Cells[15].Value.ToString();
            ftinfostr.tesrangemin = parent.dataGridView1.Rows[j].Cells[16].Value.ToString();

            ftinfostr.red = parent.dataGridView1.Rows[j].Cells[17].Value.ToString();
            ftinfostr.pitch = parent.dataGridView1.Rows[j].Cells[18].Value.ToString();
            ftinfostr.yellow = parent.dataGridView1.Rows[j].Cells[19].Value.ToString();
           
            string tinfoid = parent.dataGridView1.Rows[j].Cells[0].Value.ToString();
            if (parameter.fuc == "update")
            {
                ftinfostr.ftesttimes = this.textBox_ftesttimes.Text.Trim();
                ftinfostr.fsample = this.textBox2.Text.Trim();
                ftinfostr.wavemode = this.textBox3.Text.Trim();
                ftinfostr.wavemajor = this.textBox4.Text.Trim();
                ftinfostr.waveminor = this.textBox5.Text.Trim();
                ftinfostr.formulaC = this.textBox6.Text.Trim();
                ftinfostr.formulaB = this.textBox7.Text.Trim();
                ftinfostr.formulaA = this.textBox8.Text.Trim();
                ftinfostr.interdilute = this.textBox9.Text.Trim();
                ftinfostr.unit = this.textBox10.Text.Trim();
                ftinfostr.standards = this.textBox11.Text.Trim();
                ftinfostr.comparemode = this.textBox12.Text.Trim();
                ftinfostr.comparemin = this.textBox13.Text.Trim();
                ftinfostr.comparemax = this.textBox14.Text.Trim();
                ftinfostr.tesrangemax = this.textBox15.Text.Trim();
                ftinfostr.tesrangemin = this.textBox16.Text.Trim();
                ftinfostr.red = this.textBox_red.Text.Trim();
                ftinfostr.pitch = this.textBox1_pitch.Text.Trim();
                ftinfostr.yellow = this.textBox1_yellow.Text.Trim();
                strsql = " UPDATE ftinfor set " +
                           " ftestitems = '" + ftinfostr.ftesttimes + "'," +
                           " fsample = '" + ftinfostr.fsample + "'," +
                           " wavemode = '" + ftinfostr.wavemode + "'," +
                           " wavemajor = '" + ftinfostr.wavemajor + "'," +
                           " waveminor = '" + ftinfostr.waveminor + "'," +
                           " formulaC = '" + ftinfostr.formulaC + "'," +
                           " formulaB = '" + ftinfostr.formulaB + "'," +
                           " formulaA = '" + ftinfostr.formulaA + "'," +
                           " interdilute = '" + ftinfostr.interdilute + "'," +
                           " unit = '" + ftinfostr.unit + "'," +
                           " standards = '" + ftinfostr.standards + "'," +
                            " comparemode = '" + ftinfostr.comparemode + "'," +
                           " comparemin = '" + ftinfostr.comparemin + "'," +
                           " comparemax = '" + ftinfostr.comparemax + "'," +
                           " testrangemax = '" + ftinfostr.tesrangemax + "'," +
                           " testrangemin = '" + ftinfostr.tesrangemin + "'," +
                           " alarm_red = '" + ftinfostr.red + "'," +
                           " alarm_pitch = '" + ftinfostr.pitch + "'," +
                           " alarm_yellow = '" + ftinfostr.yellow + "'" +
                           " WHERE id = '" + tinfoid + "'";


            }
            else
            {
                ftinfostr.ftesttimes = this.textBox_ftesttimes.Text.Trim();
                ftinfostr.fsample = this.textBox2.Text.Trim();
                ftinfostr.wavemode = this.textBox3.Text.Trim();
                ftinfostr.wavemajor = this.textBox4.Text.Trim();
                ftinfostr.waveminor = this.textBox5.Text.Trim();
                ftinfostr.formulaC = this.textBox6.Text.Trim();
                ftinfostr.formulaB = this.textBox7.Text.Trim();
                ftinfostr.formulaA = this.textBox8.Text.Trim();
                ftinfostr.interdilute = this.textBox9.Text.Trim();
                ftinfostr.unit = this.textBox10.Text.Trim();
                ftinfostr.standards = this.textBox11.Text.Trim();
                ftinfostr.comparemode = this.textBox12.Text.Trim();
                ftinfostr.comparemin = this.textBox13.Text.Trim();
                ftinfostr.comparemax = this.textBox14.Text.Trim();
                ftinfostr.tesrangemax = this.textBox15.Text.Trim();
                ftinfostr.tesrangemin = this.textBox16.Text.Trim();
                ftinfostr.red = this.textBox_red.Text.Trim();
                ftinfostr.pitch = this.textBox1_pitch.Text.Trim();
                ftinfostr.yellow = this.textBox1_yellow.Text.Trim();
            
               

                strsql = "Insert Into " +
                         "ftinfor (ftestitems,fsample,wavemode,wavemajor,waveminor,formulaC,formulaB,formulaA,interdilute,unit,standards,comparemode,comparemin,comparemax,testrangemax,testrangemin,alarm_red, alarm_pitch, alarm_yellow)"
                         + "values('" + ftinfostr.ftesttimes + "','" + ftinfostr.fsample + "','" + ftinfostr.wavemode + "','" + ftinfostr.wavemajor + "','" + ftinfostr.waveminor + "','" + ftinfostr.formulaC + "','" + ftinfostr.formulaB + "','" + ftinfostr.formulaA + "',"
                         + " '" + ftinfostr.interdilute + "','" + ftinfostr.unit + "', '" + ftinfostr.standards + "','" + ftinfostr.comparemode + "',"
                          + " '" + ftinfostr.comparemin + "','" + ftinfostr.comparemax + "',  '" + ftinfostr.tesrangemax + "', '" + ftinfostr.tesrangemin + "',"
                          +"'"+ ftinfostr.red +"', '"+ftinfostr.pitch+"', '"+ftinfostr.yellow+"')";

            }
            dbMySql.Open(databaseName);
            dbMySql.ExcuteNonQuery(databaseName, strsql);
            dbMySql.Close(databaseName);
            parent.dataGridView1.Refresh();
            parent.LoadConfigInfo();
            this.Close();
            
        }

        private void AddCheckLoad(object sender, EventArgs e)
        {
            if (parameter.fuc == "update")
            {
                int j = parent.getRow();//获取datagriadview行号
                //DataGridViewRow r = parent.dataGridView1.Rows[rowNum];
                this.textBox_ftesttimes.Text = parent.dataGridView1.Rows[j].Cells[1].Value.ToString();
                textBox2.Text = parent.dataGridView1.Rows[j].Cells[2].Value.ToString();
                textBox3.Text = parent.dataGridView1.Rows[j].Cells[3].Value.ToString();
                textBox4.Text = parent.dataGridView1.Rows[j].Cells[4].Value.ToString();
                textBox5.Text = parent.dataGridView1.Rows[j].Cells[5].Value.ToString();
                textBox6.Text = parent.dataGridView1.Rows[j].Cells[6].Value.ToString();
                textBox7.Text = parent.dataGridView1.Rows[j].Cells[7].Value.ToString();
                textBox8.Text = parent.dataGridView1.Rows[j].Cells[8].Value.ToString();
                textBox9.Text = parent.dataGridView1.Rows[j].Cells[9].Value.ToString();
                textBox10.Text = parent.dataGridView1.Rows[j].Cells[10].Value.ToString();
                textBox11.Text = parent.dataGridView1.Rows[j].Cells[11].Value.ToString();
                textBox12.Text = parent.dataGridView1.Rows[j].Cells[12].Value.ToString();
                textBox13.Text = parent.dataGridView1.Rows[j].Cells[13].Value.ToString();
                textBox14.Text = parent.dataGridView1.Rows[j].Cells[14].Value.ToString();
                textBox15.Text = parent.dataGridView1.Rows[j].Cells[15].Value.ToString();
                textBox16.Text = parent.dataGridView1.Rows[j].Cells[16].Value.ToString();

                textBox_red.Text = parent.dataGridView1.Rows[j].Cells[17].Value.ToString();
                textBox1_pitch.Text = parent.dataGridView1.Rows[j].Cells[18].Value.ToString();
                textBox1_yellow.Text = parent.dataGridView1.Rows[j].Cells[19].Value.ToString();

                IDNum = int.Parse(parent.dataGridView1.Rows[j].Cells["id"].Value.ToString());
            }
        }

        private void PitchTextChanged(object sender, EventArgs e)
        {
            double redal = double.Parse(this.textBox_red.Text.ToString());
            double pitchal = double.Parse(this.textBox1_pitch.Text.ToString());
            if (pitchal > redal)
            {
                MessageBox.Show("请输入小于红色报警并大于黄色报警的数据");
                textBox1_pitch.Text = "0.0";
            }
        }

        private void yellow_TextChanged(object sender, EventArgs e)
        {
            double yellowal =  double.Parse(this.textBox1_yellow.Text.ToString());
            double pitchal = double.Parse(this.textBox1_pitch.Text.ToString());
            if (yellowal > pitchal )
            {
                MessageBox.Show("请输入小于粉红色报警的数据");
                textBox1_yellow.Text = "0.0";
            }
        }
    }
}
