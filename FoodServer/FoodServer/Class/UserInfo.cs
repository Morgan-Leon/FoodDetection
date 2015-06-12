using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FoodServ.Class
{
   public class MsgPro
    {


        /// <summary>
        /// 数据头  
        /// 数据长度
        /// 业务数据类型
        /// 监测仪器接入码
        /// </summary>
        public struct MSGHEAD
        {
            UInt32 Msg_Length; //msg id
            UInt16 Msg_ID;
            UInt32 Msg_GNSSCenter;

        };

        /// <summary>
        /// 定义结构体
        /// 
        /// </summary>
       // [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct MSG
        {
            UInt32 flag;
            MSGHEAD msg_header;
            byte[] msg_body;
            UInt16 crc;
            UInt32 endflag;
        };

      // public UInt32 flag { get; set; }
       public MSG msg { get; set; }
      // public UInt32 endflag { get; set; }

      // public CommandTypes commandsTypes { get; set; }
    }
}
