using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHCS
{
    // 网关与服务器通信的命令
    static class COMMAND
    {
        public const byte UPLOAD_DATA = 0x05;    // 上传数据

        public const byte UPLOAD_STATUS = 0x06;  // 上传状态
		
		public const byte GATEWAY_VERIFY = 0x23;  //网关身份验证
		
		public const byte GATEWAY_REGISTER = 0x24; //网关注册
		
		public const byte CONTROL = 0x67; //下发控制命令
		
		public const byte CONTROL_ACK = 0x68; //对控制的回应

        public const byte CONFIG_SYN_TOPDOWN = 0x69;  //服务器向网关同步配置信息

        public const byte CONFIG_SYN_TOPDOWN_ACK = 0X70; //网关回复服务器同步结果
    }

    // 受控设备类型
    static class DEVICESTYPE
    {
        public const string SHI_LIAN_FENG_JI        = "6";
        public const string JUAN_MO_QI1             = "4";
        public const string JUAN_MO_QI2             = "5";
        public const string SHI_LIAN_SHUI_BENG      = "7";
        public const string GUAN_DAO_XUN_HUAN_BENG  = "8";
        public const string CO2_DIAN_CI_FA          = "9";
        public const string YING_YANG_YE_CHI_BENG   = "10";
        public const string GUAN_GAI_DIAN_CI_FA     = "11";
    }


    // 受控设备状态
    static class DEVICESSTATUS
    {
        public const byte MANUAL_OPEN = 0;    // 现场手动开
        public const byte MANUAL_CLOSE = 1;   // 现场手动关

        public const byte REMOTE_OPEN = 2;    // 远程开
        public const byte REMOTE_CLOSE = 3;   // 远程关

        public const byte NO_DEVICE = 4;      // 无设备

        public static string[] StatusString = new string[] { "现场手动开 ", "现场手动关", "远程开", "远程关", "无设备" };
             
    }    
}
