using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using Lib.Xml;
using Lib.Expression;

namespace LJTH.BusinessIndicators.Web.SystemConfiguration
{
    public partial class MXLTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<C_System> sysList = StaticResource.Instance.SystemList.Where(p => p.Category != 3).ToList();
                ddlSystem.DataSource = sysList.OrderBy(or => or.Sequence).ToList();
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
                CheckXML();
            }
        }

     


        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckXML();

        }
        List<XElement> ReturnListXML;//补回情况
        List<XElement> MisstargetListXML;//未完成
        string ReturnExpression;//补回情况公式
        string ReturnSonExpression;//未到期公式
        string error = "";
        C_System model = new C_System();
        public void CheckXML()
        {
            error = "";
            string sysid = ddlSystem.SelectedValue;
             model = C_SystemOperator.Instance.GetSystem(Guid.Parse(sysid));

            XElement XML = model.Configuration;//取出xml
            returnxml.Value = XML.ToString();
            Check(XML);

            if (error != "")
            {
                this.romerror.InnerText = "";
                this.romerror.InnerText = "该系统公式错误";
                Response.Write("该公式错误:" + error);
            }
            else
            {
                this.romerror.InnerText = "";
                this.romerror.InnerText = "该系统公式正确!";
            }
        }
        //验证xml
        private void Check(XElement xml)
        {
            XElement XML = xml;
            ReturnListXML = XML.Elements("MisstargetReturn").Elements("Group").Elements("Counter").ToList();//取出补回情况xml
            if (ReturnListXML.Count > 0)//判断补回情况的xml是否存在
            {
                for (int i = 0; i < ReturnListXML.Count; i++)
                {
                    List<XElement> ReturnListXMLi = ReturnListXML[i].Elements("Counter").ToList();
                    ReturnExpression = ReturnListXML[i].GetAttributeValue("Expression", "");
                    if (ReturnListXMLi.Count > 0)//未到期下还有细分的xml
                    {
                        for (int a = 0; a < ReturnListXMLi.Count; a++)
                        {
                            ReturnSonExpression = ReturnListXMLi[a].GetAttributeValue("Expression", "");
                            //对未到期的子项进行测试
                            test(ReturnSonExpression);
                        }
                    }

                    //对未到期进行测试
                    test(ReturnExpression);
                }
            }

            MisstargetListXML = XML.Elements("Misstarget").Elements("Group").Elements("Counter").ToList();//取出未完成情况xml
            if (MisstargetListXML.Count > 0)
            {
                for (int i = 0; i < MisstargetListXML.Count; i++)
                {
                    string MisstargetTitle = MisstargetListXML[i].GetAttributeValue("Title", "");//取出未完成XML中的title
                    string MisstargetExpression = MisstargetListXML[i].GetAttributeValue("Expression", "");//取出未完成XML中的Expression
                    if (model.Category != 3 && model.Category != 4)//因为直管公司的未完成公式比较简单   不需要进行处理.
                    {
                        //解析title,因为title的格式为aaa{bb}c{ddd},所以用{和}吧字符串分割,并且保留有效数据
                        string[] Arr = MisstargetTitle.Split(new char[2] { '{', '}' });
                        ArrayList Brr = new ArrayList();
                        for (int a = 0; a < Arr.Length; a++)
                        {
                            if (Arr[a].Length > 2)
                            {
                                if (Arr[a].Substring(0, 2) == "上单" || Arr[a].Substring(0, 2) == "上双" || Arr[a].Substring(0, 2) == "本月")
                                {
                                    Brr.Add(Arr[a].Replace("上单", "").Replace("上双", "").Replace("本月", ""));
                                }
                            }
                        }
                        //对Brr中的数据进行测试i
                        for (int aa = 0; aa < Brr.Count; aa++)
                        {
                            test(Brr[aa].ToString());
                        }
                    }
                    //对MisstargetExpression进行测试
                    test(MisstargetExpression);
                }
            }

        }

        private void test(string str)
        {
            try
            {
                ExpressionParser tesdt = new ExpressionParser("a");
                bool s = tesdt.CacluateCondition(str);
            }
            catch (Exception)
            {
                error = str;
            }
        }

        protected void Unnamed1_Click(object sender, EventArgs e)
        {
            error = "";
            string xml = returnxml.Value;
            XElement x = null;
            if (xml!="")
            {
                try
                {
                    x = XElement.Parse(xml);
                    Check(x);
                }
                catch (Exception)
                {

                    error = "xml格式有问题";
                }
            }
            if (error != "")
            {
                Response.Write("<script>alert('该公式错误:" + error + "')</script>"); 
            }
            else
            {
                Response.Write("<script>alert('该公式正确')</script>"); 
            }
           
        }



        private string XMLTranslation(string str)
        {
            str = str.Replace("&amp;", "&").Trim(); ;
            str = str.Replace("&lt;", "<").Trim(); ;
            str = str.Replace("&gt;", ">").Trim(); ;
            str = str.Replace("&quot;", "\"").Trim(); ;
            str = str.Replace("&apos;", "'").Trim(); ;
            return str;
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            string xml = returnxml.Value;
            error = "";
            XElement x = null;
            if (xml!="")
            {
                x = XElement.Parse(xml);
                Check(x);
            }
            else
            {
                Response.Write("<script>alert('数据不能为空')</script>"); 
            }
            
            if (error!="")
            {
                Response.Write("<script>alert('该公式错误,请检查!')</script>"); 
            }
            else
            {
                string sysid = ddlSystem.SelectedValue;
                model = C_SystemOperator.Instance.GetSystem(Guid.Parse(sysid));
                model.Configuration = x;
                C_SystemOperator.Instance.UpdateSystem(model);
                Response.Write("<script>alert('保存成功')</script>"); 
            }
        }
    }
}