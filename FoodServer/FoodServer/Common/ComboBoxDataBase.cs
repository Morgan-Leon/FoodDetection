using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

using GHCS.Common;

namespace GHCS.Common
{
    class ComboBoxDataBase
    {
        /// <summary>
        /// 列表控件绑定数据库中的内容
        /// </summary>
        /// <param name="box"></param>
        /// <param name="database"></param>
        /// <param name="sql"></param>
        public void BindDataBase(ref ComboBox box,ref IDataBase database,string sql)
        {
            if (box == null || database == null)
            {
                return;
            }

            box.Items.Clear();
            // 从数据库中获取数据并放入到列表控件中
            List<string> list = new List<string>();
            database.QueryTable(sql,list,1);

            for (int i = 0; i < list.Count;i++ )
            {
                box.Items.Add(list[i]);
            }
        }
    }
}
