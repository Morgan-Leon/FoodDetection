using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FoodServer.realmonitor
{
   public class DataCollectton
    {
        string m_csName;
        string m_csAddress;
        string m_csDeviceType;
        string m_csSoftwareVersion;

        //中心位置
        Point m_point;
        Size m_site;	//在整个窗口中的比例
        bool m_bIsSelect;

       public DataCollectton()
        {
            m_bIsSelect = true;
        }

       public DataCollectton(string csName, string csAddress, string csDeviceType, string csSoftwareVersion)
        {
            m_csName = csName;

            m_csAddress = csAddress;
            m_csDeviceType = csDeviceType;

            m_csSoftwareVersion = csSoftwareVersion;
            m_point.X = 0;
            m_point.Y = 0;
            m_bIsSelect = true;
        }
    }
}
