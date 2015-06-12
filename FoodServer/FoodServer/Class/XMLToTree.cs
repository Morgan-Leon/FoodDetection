using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace TreeExXML
{
    class TreeExXMLCls
    {
        private TreeView thetreeview;
        private string xmlfilepath;
        XmlTextWriter textWriter;
        XmlNode Xmlroot;
        XmlDocument textdoc;

        public TreeExXMLCls()
        {
            //----构造函数
            textdoc = new XmlDocument();

        }

        ~TreeExXMLCls()
        {
            //----析构函数

        }

        #region 遍历treeview并实现向XML的转化
        /// <summary>   
        /// 遍历treeview并实现向XML的转化
        /// </summary>   
        /// <param name="TheTreeView">树控件对象</param>   
        /// <param name="XMLFilePath">XML输出路径</param>   
        /// <returns>0表示函数顺利执行</returns>   

        public int TreeToXML(TreeView TheTreeView, string XMLFilePath)
        {
            //-------初始化转换环境变量
            thetreeview = TheTreeView;
            xmlfilepath = XMLFilePath;
            textWriter = new XmlTextWriter(xmlfilepath, null);
           
            //-------创建XML写操作对象
            textWriter.Formatting = Formatting.Indented;

            //-------开始写过程，调用WriteStartDocument方法
            textWriter.WriteStartDocument();

            //-------写入说明
            textWriter.WriteComment("this XML is created from a tree");
            textWriter.WriteComment("By jaccen");

            //-------添加第一个根节点
            textWriter.WriteStartElement("TreeExXMLCls");
            textWriter.WriteEndElement();

            //------ 写文档结束，调用WriteEndDocument方法
            textWriter.WriteEndDocument();

            //-----关闭输入流
            textWriter.Close();

            //-------创建XMLDocument对象
            textdoc.Load(xmlfilepath);

            //------选中根节点
            XmlElement Xmlnode = textdoc.CreateElement(thetreeview.Nodes[0].Text);
            Xmlroot = textdoc.SelectSingleNode("TreeExXMLCls");

            //------遍历原treeview控件，并生成相应的XML
            TransTreeSav(thetreeview.Nodes, (XmlElement)Xmlroot);


            return 0;


        }

        private int TransTreeSav(TreeNodeCollection nodes, XmlElement ParXmlnode)
        {

            //-------遍历树的各个故障节点，同时添加节点至XML
            XmlElement xmlnode;
            Xmlroot = textdoc.SelectSingleNode("TreeExXMLCls");

            foreach (TreeNode node in nodes)
            {
                xmlnode = textdoc.CreateElement(node.Text);
                ParXmlnode.AppendChild(xmlnode);

                if (node.Nodes.Count > 0)
                {
                    TransTreeSav(node.Nodes, xmlnode);
                }
            }
            textdoc.Save(xmlfilepath);

            return 0;
        }

        #endregion

        #region 遍历XML并实现向tree的转化
        /// <summary>   
        /// 遍历treeview并实现向XML的转化
        /// </summary>   
        /// <param name="XMLFilePath">XML输出路径</param>   
        /// <param name="TheTreeView">树控件对象</param>   
        /// <returns>0表示函数顺利执行</returns>   

        public int XMLToTree(string XMLFilePath, TreeView TheTreeView)
        {
            //-------重新初始化转换环境变量
            thetreeview = TheTreeView;
            xmlfilepath = XMLFilePath;

            //-------重新对XMLDocument对象赋值
            textdoc.Load(xmlfilepath);

            XmlNode root = textdoc.SelectSingleNode("TreeExXMLCls");

            foreach (XmlNode subXmlnod in root.ChildNodes)
            {
                TreeNode trerotnod = new TreeNode();
                trerotnod.Text = subXmlnod.Name;
                thetreeview.Nodes.Add(trerotnod);
                TransXML(subXmlnod.ChildNodes, trerotnod);

            }

            return 0;
        }

        private int TransXML(XmlNodeList Xmlnodes, TreeNode partrenod)
        {
            //------遍历XML中的所有节点，仿照treeview节点遍历函数
            foreach (XmlNode xmlnod in Xmlnodes)
            {
                TreeNode subtrnod = new TreeNode();
                subtrnod.Text = xmlnod.Name;
                partrenod.Nodes.Add(subtrnod);

                if (xmlnod.ChildNodes.Count > 0)
                {
                    TransXML(xmlnod.ChildNodes, subtrnod);
                }
            }

            return 0;

        }

        #endregion















        //添加
        public void AddTreeNode(string XMLFilePath, string fathernode, string newnode)
        {
            xmlfilepath = XMLFilePath;

            //-------重新对XMLDocument对象赋值
            textdoc.Load(xmlfilepath);
            XmlNode rootXml = textdoc.SelectSingleNode("TreeExXMLCls");   //查询XML文件的根节点("siteMapPath")
            XmlNode xn = rootXml.SelectSingleNode(fathernode);   //查询节点("siteMap")
            XmlElement xe = textdoc.CreateElement(newnode);    //创建节点("siteMap")
            xn.AppendChild(xe);
            textdoc.Save(XMLFilePath);
        }

        //This function is called recursively until all nodes are loaded
        public void addTreeNode(string XMLFilePath, string treeNode)
        {
            //-------重新初始化转换环境变量
          
            xmlfilepath = XMLFilePath;

            //-------重新对XMLDocument对象赋值
            textdoc.Load(xmlfilepath);
            XmlNode rootXml = textdoc.SelectSingleNode("TreeExXMLCls");   //查询XML文件的根节点("siteMapPath")
            XmlNode xn = rootXml.SelectSingleNode("全部采集点");   //查询节点("siteMap")
            XmlElement xe = textdoc.CreateElement(treeNode);    //创建节点("siteMap")
            xn.AppendChild(xe);
         //  rootXml.InsertAfter(xe, xn);    //将新建的节点xe放置于xn节点后   
            textdoc.Save(XMLFilePath);
           
        }

        public void addTreeNode(string XMLFilePath, string NewNode,string node)
        {
            //-------重新初始化转换环境变量

            xmlfilepath = XMLFilePath;

            //-------重新对XMLDocument对象赋值
            textdoc.Load(xmlfilepath);
            XmlNode rootXml = textdoc.SelectSingleNode("TreeExXMLCls");   //查询XML文件的根节点("siteMapPath")
            if (rootXml.HasChildNodes)
            {
                foreach (XmlNode nl  in rootXml.ChildNodes)
                {
                    XmlNode xn = nl.SelectSingleNode(node);   //查询节点("siteMap")
                    if (xn != null)
                    {
                        XmlElement xe = textdoc.CreateElement(NewNode);    //创建节点("siteMap")
                        xn.AppendChild(xe);
                    }
                }
            }
            
           
            //  rootXml.InsertAfter(xe, xn);    //将新建的节点xe放置于xn节点后   
            textdoc.Save(XMLFilePath);

        }
        public bool UpdateXmlNodeByXPath(string xmlFileName, string newXpath, string xpath)
        {
            bool isSuccess = false;
            xmlfilepath = xmlFileName;
            try
            {
                textdoc.Load(xmlFileName); //加载XML文档
                XmlNode rootXml = textdoc.SelectSingleNode("TreeExXMLCls");   //查询XML文件的根节点("siteMapPath")
                XmlNodeList node = rootXml.ChildNodes;

               
                foreach (XmlNode xmln in node)
                {
                    XmlNode node_one = xmln.SelectSingleNode(xpath);
                    if (node_one != null)
                    {
                        xmln.RemoveChild(node_one);
                        XmlElement xe = textdoc.CreateElement(newXpath);
                        xmln.AppendChild(xe);
                    }
                    else
                    {
                        XmlNodeList nodes = xmln.ChildNodes;
                        foreach (XmlNode item in nodes)
                        {
                            XmlNode node_two = item.SelectSingleNode(xpath);
                            if (node_two != null)
                            {
                                XmlElement xe = textdoc.CreateElement(newXpath);
                                xmln.AppendChild(xe);
                            }
                        }
                    }
                }
               
                textdoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;

            }
            catch (Exception ex)
            {
                throw ex; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }
        public  bool DeleteXmlNodeByXPath(string xmlFileName,TreeNode fathernode, string xpath)
        {
            bool isSuccess = false;
            xmlfilepath = xmlFileName;
            try
            {
                textdoc.Load(xmlFileName); //加载XML文档
                XmlNode rootXml = textdoc.SelectSingleNode("TreeExXMLCls");   //查询XML文件的根节点("siteMapPath")
                XmlNodeList node = rootXml.ChildNodes;
                
                if (fathernode == null)
                {
                    foreach (XmlNode xmln in node)
                    {
                        XmlNode node_one = xmln.SelectSingleNode(xpath);
                        if (node_one!=null)
                        {
                            xmln.RemoveChild(node_one);
                        }
                        else
                        {
                            XmlNodeList nodes = xmln.ChildNodes;
                            foreach (XmlNode item in nodes)
                            {
                                XmlNode node_two = item.SelectSingleNode(xpath);
                                if (node_two != null)
                                {
                                    item.RemoveChild(node_two);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (XmlNode xmln in node)
                    {
                        XmlNode xmlNode = xmln.SelectSingleNode(fathernode.Text);
                        if (xmlNode != null)
                        {
                            XmlNode cnode = xmlNode.SelectSingleNode(xpath);
                            if (cnode != null)
                            {
                                //遍历xpath节点中的所有属性
                                xmlNode.RemoveChild(cnode);
                            }
                        }
                    }
                }
                textdoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
               
            }
            catch (Exception ex)
            {
                throw ex; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }
        public List<string> GetTreeNode(string XMLFilePath, string treeNode)
        {
            List<string> ls = new List<string>();
            xmlfilepath = XMLFilePath;

            //-------重新对XMLDocument对象赋值
            textdoc.Load(xmlfilepath);
            XmlNode rootXml = textdoc.SelectSingleNode("TreeExXMLCls");   //查询XML文件的根节点("siteMapPath")
            XmlNode xn = rootXml.SelectSingleNode(treeNode);   //查询节点("siteMap")
            foreach (XmlNode subXmlnod in xn.ChildNodes)
            {
                TreeNode trerotnod = new TreeNode();
                trerotnod.Text = subXmlnod.Name;
                ls.Add(subXmlnod.Name);
            }
            return ls;
        }
    }
}