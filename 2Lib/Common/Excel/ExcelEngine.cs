using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Aspose.Cells;
using System.Data;
using System.IO;
using System.Web;
using License = Aspose.Cells.License;
using System.Configuration;
using System.Reflection;

namespace LJTH.BusinessIndicators.Common
{
    public class ExcelEngine
    {
        static License _license = null;
        /// <summary>
        /// 缺省构造方法
        /// </summary>
        public ExcelEngine()
        {
            //注册Excel组件
            if (_license == null)
            {
                string licenseFile = HttpContext.Current.Server.MapPath("/Aspose.Total.lic");

                _license = new License();
                _license.SetLicense(licenseFile);
            }
        }

        public ExcelEngine(string AsposePath)
        {
            //注册Excel组件 ，这是为了给的服务用的构造函数
            if (_license == null)
            {
                string licenseFile = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + ConfigurationManager.AppSettings["AsposePath"];

                _license = new License();
                _license.SetLicense(licenseFile);
            }
        }


        /// <summary>
        /// 导出数据生成文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExportExcel(DataTable dt, string templetePath, string templeteName, string filePath, string fileName)
        {

            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            designer.Workbook = new Workbook(fileStream);

            designer.SetDataSource(dt);

            designer.Process();

            string fileToSave = System.IO.Path.Combine(filePath, fileName);
            if (File.Exists(fileToSave))
            {
                File.Delete(fileToSave);
            }

            designer.Workbook.Save(fileToSave, SaveFormat.Xlsx);

            fileStream.Close();
            fileStream.Dispose();
        }



        /// <summary>
        /// 导出数据生成文件流
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="templetePath"></param>
        /// <param name="templeteName"></param>
        /// <returns></returns>
        public MemoryStream ExportExcel(DataTable dt, string templetePath, string templeteName)
        {

            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {


                designer.Workbook = new Workbook(fileStream);

                designer.SetDataSource(dt);

                designer.Process();

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();

                return stream;
            }
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.Buffer = true;
            //HttpContext.Current.Response.Charset = "utf-8";
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            //HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            //HttpContext.Current.Response.End();

        }

        /// <summary>
        /// 导出数据生成文件流下载
        /// </summary>
        /// <param name="list">导出的数据列表</param>
        /// <param name="listName">列表名</param>
        /// <param name="templetePath">模版路径</param>
        /// <param name="templeteName">模版名称</param>
        public MemoryStream ExportExcel<T>(List<T> list, string listName, string templetePath, string templeteName)
        {

            WorkbookDesigner designer = new WorkbookDesigner();

            string path = System.IO.Path.Combine(templetePath, templeteName);
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                designer.Workbook = new Workbook(fileStream);

                designer.SetDataSource(listName, list);

                designer.Process();

                //designer.Workbook.CalculateFormula();

                MemoryStream stream = designer.Workbook.SaveToStream();
                fileStream.Close();
                fileStream.Dispose();

                return stream;

            }
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.Buffer = true;
            //HttpContext.Current.Response.Charset = "utf-8";
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
            //HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            //HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            //HttpContext.Current.Response.End();

        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="workbook">待导入数据的工作簿</param>
        /// <returns></returns>
        public DataSet ImportDataFromWorkbook(Workbook workbook)
        {
            DataSet ds = new DataSet();
            foreach (Worksheet item in workbook.Worksheets)
            {
                Worksheet worksheet = item;

                DataTable dataTable = new DataTable();

                if (worksheet.Cells.MaxRow <= 0 || worksheet.Cells.MaxColumn <= 0)
                    continue;
                dataTable = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxRow + 1,
                             worksheet.Cells.MaxColumn + 1);
                ds.Tables.Add(dataTable);
            }
            return ds;
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public DataSet ImportDataFromWorkbook(string filePath, string fileName)
        {
            string path = Path.Combine(filePath, fileName);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            Workbook workbook = new Workbook(fileStream);

            return ImportDataFromWorkbook(workbook);
        }

        ///// <summary>
        ///// 创建日记账导入模版
        ///// </summary>
        ///// <typeparam name="T">银行数据源</typeparam>
        ///// <param name="list"></param>
        ///// <param name="getSheets"></param>
        ///// <param name="templetePath">模版路径</param>
        ///// <param name="templeteName">模版文件名</param>
        ///// <param name="fileName">文件名</param>
        //public void CreateDayBillTemplete<T>(List<T> list, Func<List<T>, WorksheetCollection, WorksheetCollection> getSheets, string templetePath, string templeteName, string fileName)
        //{

        //    string path = System.IO.Path.Combine(templetePath, templeteName); //路径
        //    Workbook workbook = new Workbook();  //工作博
        //    workbook.Open(path); //打开
        //    WorksheetCollection sheets = workbook.Worksheets; //工作表

        //    sheets = getSheets(list, sheets);

        //    AddCustomProperties(workbook, 1, 123);


        //    MemoryStream stream = workbook.SaveToStream();

        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.Buffer = true;
        //    HttpContext.Current.Response.Charset = "utf-8";
        //    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
        //    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
        //    HttpContext.Current.Response.ContentType = "application/ms-excel";
        //    HttpContext.Current.Response.BinaryWrite(stream.ToArray());
        //    HttpContext.Current.Response.End();

        //}


        public int GetCustomProperty(Worksheet worksheet, string name)
        {
            if (worksheet.CustomProperties[name] != null)
            {
                string s = worksheet.CustomProperties[name].Value;
                return string.IsNullOrEmpty(s) == false ? Convert.ToInt32(s) : 0;
            }
            else
            {
                return 0;
            }
        }

        public Guid GetGuidCustomProperty(Worksheet worksheet, string name)
        {
            if (worksheet.CustomProperties[name] != null)
            {
                string s = worksheet.CustomProperties[name].Value;
                return string.IsNullOrEmpty(s) == false ? Guid.Parse(s) : Guid.Empty;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 获取自定义信息，并验证是否每个工作表中的自定义信息都相同
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="templateType"></param>
        /// <param name="versionId"></param>
        /// <param name="periodId"></param>
        /// <param name="projectId"></param>
        /// <param name="editionId"></param>
        /// <returns></returns>
        public bool GetCustomProperties(Workbook workbook,  out int versionId, out Guid projectId)
        {
            //待返回的值
            versionId = 0;
            projectId = Guid.Empty;

            //单一工作表的自定义属性
            int versionId1 = 0;
            Guid projectId1 = Guid.Empty;

            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                Worksheet worksheet = workbook.Worksheets[i];

                versionId1 = GetCustomProperty(worksheet, ExcelConst.WorksheetPropertyVersion);
                projectId1 = GetGuidCustomProperty(worksheet, ExcelConst.WorksheetPropertyProject);

                if (versionId1 > 0)
                {
                    //判断每个工作表中的自定义属性是否都一样
                    if (i > 0)
                    {
                        if (versionId1 != versionId
                            || projectId1 != projectId)
                        {
                            return false;
                        }
                    }

                    versionId = versionId1;
                    projectId = projectId1;
                }
            }

            return true;
        }

        public string GetStringCustomProperty(Worksheet worksheet, string key)
        {
            if (worksheet.CustomProperties[key] != null)
            {
                return worksheet.CustomProperties[key].Value;
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 设置当前工作表的自定义属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetCustomProperty(Worksheet worksheet, string name, string value)
        {
            if (worksheet.CustomProperties[name] != null)
            {
                worksheet.CustomProperties[name].Value = value;
            }
            else
            {
                worksheet.CustomProperties.Add(name, value);
            }
        }




    }


    public class ExcelException : Exception
    {
        public ExcelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ExcelException(string message)
            : base(message)
        {
        }
    }

    public class ExcelHelper
    {
        #region 私有成员

        Worksheet _worksheet = null;
        
        static License _license = null;

        #endregion


        
        /// <summary>
        /// 缺省构造方法
        /// </summary>
        public ExcelHelper()
        {
            //注册Excel组件
            if (_license == null)
            {
                string licenseFile = HttpContext.Current.Server.MapPath("/Aspose.Total.lic");

                _license = new License();
                _license.SetLicense(licenseFile);
            }
        }



        #region 构造方法

        public ExcelHelper(Worksheet worksheet)
        {
            this._worksheet = worksheet;

            //注册Excel组件
            if (_license == null)
            {
                string licenseFile = HttpContext.Current.Server.MapPath("/Aspose.Total.lic");

                _license = new License();
                _license.SetLicense(licenseFile);
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取一个单元格的值。文本内容的前导和后置空格及空行将被移除
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public string GetCellValue(Cell cell)
        {
            if (cell == null) return string.Empty;

            string returnValue = string.Empty;

            switch (cell.Type)
            {
                case CellValueType.IsError:
                case CellValueType.IsNull:
                    returnValue = string.Empty;
                    break;
                case CellValueType.IsBool:
                    returnValue = cell.BoolValue.ToString();
                    break;
                case CellValueType.IsDateTime:
                    returnValue = cell.DateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case CellValueType.IsNumeric:
                    returnValue = cell.DoubleValue.ToString();
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// 设置一个单元格的值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        public void SetCellValue(Cell cell, string value)
        {
            if (cell == null) return;
            if (cell.IsFormula || cell.IsErrorValue || cell.StringValue.Equals(value) == false)
            {
                cell.PutValue(value, true);
            }
        }

        /// <summary>
        /// 获取一个单元格的标注内容
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public string GetCellComment(Cell cell)
        {
            if (cell == null) return string.Empty;

            Comment comment = this._worksheet.Comments[cell.Row, cell.Column];
            return (comment != null) ? comment.Note : string.Empty;
        }

        /// <summary>
        /// 设置一个单元格的标注内容。
        /// 如果试图将一个已存在的标注的内容清空，则直接删除该批注
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="content"></param>
        public void SetCellComment(Cell cell, string content)
        {
            if (cell == null) return;

            Comment comment = this._worksheet.Comments[cell.Row, cell.Column];
            if (string.IsNullOrWhiteSpace(content))
            {
                //删除标注
                if (comment != null)
                {
                    this._worksheet.Comments.RemoveAt(cell.Row, cell.Column);
                }
            }
            else
            {
                //如果不存在标注，则新增一个
                if (comment == null)
                {
                    int commentIndex = this._worksheet.Comments.Add(cell.Row, cell.Column);
                    comment = this._worksheet.Comments[commentIndex];
                }

                //修改标注
                comment.Note = content;
            }
        }

        /// <summary>
        /// 获取一个单元格的公式内容
        /// </summary>
        /// <param name="cell"></param>
        public string GetCellFormula(Cell cell)
        {
            if (cell == null) return string.Empty;

            return cell.IsFormula ? cell.Formula : string.Empty;
        }

        /// <summary>
        /// 获取当前工作表的自定义属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetCustomProperty(string name)
        {
            if (this._worksheet.CustomProperties[name] != null)
            {
                return this._worksheet.CustomProperties[name].Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置当前工作表的自定义属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetCustomProperty(string name, string value)
        {
            if (this._worksheet.CustomProperties[name] != null)
            {
                this._worksheet.CustomProperties[name].Value = value;
            }
            else
            {
                this._worksheet.CustomProperties.Add(name, value);
            }
        }

        /// <summary>
        /// 获取一个命名工作表
        /// </summary>
        /// <param name="namePrefix"></param>
        /// <returns></returns>
        public List<Range> GetRangeByNamePrefix(string namePrefix)
        {
            List<Range> result = new List<Range>();

            Range[] ranges = this._worksheet.Workbook.Worksheets.GetNamedRanges();
            if (ranges != null)
            {
                foreach (Range range in this._worksheet.Workbook.Worksheets.GetNamedRanges())
                {
                    if (range.Worksheet == this._worksheet && range.Name.ToLower().StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(range);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 处理重复子模板。
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="source"></param>
        /// <param name="repeatTimes"></param>
        /// <returns>处理完成的全部子模版，包括源模版自身</returns>
        public List<Range> RepeatRange(Worksheet worksheet, Range source, int repeatTimes)
        {
            List<Range> result = new List<Range>();

            if (source.Worksheet != worksheet) return result;
            if (source == null || source.RowCount == 0 || source.ColumnCount == 0) return result;
            if (repeatTimes == 0) return result;

            //源区域是待处理的第一个区域
            result.Add(source);

            //判断子模版是行模版还是列模版
            bool isRowTemplate = source.FirstColumn == 0 && source.ColumnCount >= worksheet.Cells.MaxColumn;

            //根据模版类型复制子模版
            if (isRowTemplate)
            {
                //纵向复制模版
                for (int i = 0; i < repeatTimes - 1; i++)
                {
                    int rowStart = source.FirstRow + source.RowCount * (i + 1);
                    worksheet.Cells.InsertRows(rowStart, source.RowCount, true);
                    Range target = worksheet.Cells.CreateRange(rowStart, 0, source.RowCount, source.ColumnCount);
                    target.Copy(source);

                    result.Add(target);
                }
            }
            else
            {
                //横向复制模版
                for (int i = 0; i < repeatTimes - 1; i++)
                {
                    int colStart = source.FirstColumn + source.ColumnCount * (i + 1);
                    worksheet.Cells.InsertColumns(colStart, source.ColumnCount, true);
                    Range target = worksheet.Cells.CreateRange(0, colStart, source.RowCount, source.ColumnCount);
                    target.Copy(source);

                    result.Add(target);
                }
            }

            return result;
        }

        /// <summary>
        /// 保护当前工作表
        /// </summary>
        /// <param name="protectPassword"></param>
        /// <param name="protectionType"></param>
        public void ProtectWorksheet(string protectPassword, ProtectionType protectionType)
        {
            //如果工作表已经是保护的，假定原密码与新密码相同
            string oldPassword = string.Empty;
            if (this._worksheet.IsProtected)
            {
                oldPassword = protectPassword;
            }

            this._worksheet.Protect(protectionType, protectPassword, oldPassword);
        }

        /// <summary>
        /// 设置当前工作表为完全只读工作表
        /// </summary>
        public void SetAsReadonly()
        {
            //禁止修改格式
            Protection protection = this._worksheet.Protection;

            protection.AllowDeletingColumn = false;
            protection.AllowDeletingRow = false;
            protection.AllowEditingContent = false;
            protection.AllowEditingObject = false;
            protection.AllowEditingScenario = false;
            protection.AllowFormattingCell = false;
            protection.AllowFormattingColumn = false;
            protection.AllowFormattingRow = false;
            protection.AllowInsertingColumn = false;
            protection.AllowInsertingHyperlink = false;
            protection.AllowInsertingRow = false;

            //锁定全部工作表
            Style newStyle = new Style();
            newStyle.IsLocked = true;
            foreach (Cell cell in this._worksheet.Cells)
            {
                Style style = cell.GetStyle();
                if (style == null)
                {
                    style = newStyle;
                }
                else
                {
                    if (style.IsLocked == false) style.IsLocked = true;
                }

                cell.SetStyle(style);
            }
        }

        #endregion

        #region 公共静态方法

        /// <summary>
        /// 检查工作表中是否存在特定工作表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static bool ExistWorksheet(Workbook workbook, string sheetName)
        {
            Worksheet worksheet = workbook.Worksheets[sheetName];
            return worksheet != null;
        }

        /// <summary>
        /// 使用另一个工作表替换目标工作表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <param name="sourceSheet"></param>
        public static void ReplaceWorksheet(Workbook targetWorkbook, string targetSheetName, Worksheet sourceSheet)
        {
            if (targetWorkbook == null || string.IsNullOrEmpty(targetSheetName) || sourceSheet == null) return;

            Worksheet targetSheet = targetWorkbook.Worksheets[targetSheetName];

            if (targetSheet == null)
            {
                targetSheet = targetWorkbook.Worksheets.Add(targetSheetName);
                targetSheet.Copy(sourceSheet);
            }
            else
            {
                //targetSheet.Cells.Clear();
                targetSheet.Copy(sourceSheet);
            }
        }

        #endregion
    }


    public class ExcelConst
    {
        public static readonly string WorksheetPropertyVersion = "Version";
        public static readonly string WorksheetPropertyProject = "Project";
    }

}
