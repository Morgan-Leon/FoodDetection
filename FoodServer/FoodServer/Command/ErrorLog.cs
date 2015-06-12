using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GHCS
{
    static class ErrorLog
    {
        public static string fileName = "log.txt";

        public static void WriteLog(string msg)
        {
            try
            {
                StreamWriter logWriter = new StreamWriter(fileName, true);

                // 写入日志
                string message = DateTime.Now + "\r\n\t\t" + msg;
                logWriter.WriteLine(message);

                // 记录堆栈
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                logWriter.WriteLine("\t\tbacktrace:");
                for (int i = 0; i < st.FrameCount; i++)
                {
                    Type t = st.GetFrame(i).GetMethod().DeclaringType;
                    if (st.GetFrame(i).GetFileLineNumber() != 0)
                    {
                        logWriter.WriteLine("\t\t\t" + t.FullName + " " 
                            +st.GetFrame(i).GetMethod() + st.GetFrame(i).GetFileLineNumber());
                    }
                }

                logWriter.Close();
            }
            catch
            {
                Console.WriteLine("ErrorLog error!");
            }
        }      
    }
}
