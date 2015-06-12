using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;


//读ini文件
namespace GHCS
{
   public  class IniAc
    {
        // 声明INI文件的写操作函数 WritePrivateProfileString()

        [System.Runtime.InteropServices.DllImport("kernel32")]

        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()

        [System.Runtime.InteropServices.DllImport("kernel32")]

        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]  
      private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);  




       private string sPath = null;
       private string sFilename = "ghcs.ini";
       private string sFile = "";

       public IniAc()
       {
           sPath = Directory.GetCurrentDirectory();//获取当前目录
           sFile = sPath + "\\" + sFilename;
           HasFile();
       }
       /***
        * 构造
        * @文件路径
        * @文件名
        * ****/
       public IniAc(string path,string iniFile)
       {
           sPath = path;
           sFilename = iniFile;
           sFile = sPath + "\\" + sFilename;
           HasFile();
       }
       private void HasFile()
       {
           if (!File.Exists(sFile))
           {

               MessageBox.Show("配置文件不存在!", "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

           }
       
       }
       /*读ini
        *  @区
        *  @键名
        * */
       public string ReadValue(string section, string key)
       {

           // 每次从ini中读取多少字节

           System.Text.StringBuilder sReturn = new System.Text.StringBuilder(255);
           

           // section=配置节，key=键名，sRturn=返回值，path=路径
         

           GetPrivateProfileString(section, key, "", sReturn, 255,sFile );

           return sReturn.ToString();

       }

 
       //写ini待完成


       public void WritValue(string section, string key, string value)
       {

           // section=配置节，key=键名，value=键值，path=路径

           WritePrivateProfileString(section, key, value, sPath);

       }

       //将指定的Section中的所有Ident
       public void ReadSection(string Section, StringCollection Idents)
       {
           Byte[] Buffer = new Byte[16384];
           //Idents.Clear();

           int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
            sFile);
           //对Section进行解析
           GetStringsFromBuffer(Buffer, bufLen, Idents);
       }

       /// <summary>
       /// 读取所有section
       /// </summary>
       /// <param name="SectionList"></param>
       public void ReadSections(StringCollection SectionList)
       {
           //Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section
           byte[] Buffer = new byte[65535];
           int bufLen = 0;
           bufLen = GetPrivateProfileString(null, null, null, Buffer,
            Buffer.GetUpperBound(0), sFile);
           GetStringsFromBuffer(Buffer, bufLen, SectionList);
       }

       private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
       {
           Strings.Clear();
           if (bufLen != 0)
           {
               int start = 0;
               for (int i = 0; i < bufLen; i++)
               {
                   if ((Buffer[i] == 0) && ((i - start) > 0))
                   {
                       String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                       Strings.Add(s);
                       start = i + 1;
                   }
               }
           }
       }
    }
}
