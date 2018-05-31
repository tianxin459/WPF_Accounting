
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.Model;
using NPOI.XSSF.UserModel;
using NPOI.CSS;

namespace Accounting.Util
{
    public class ExcelUtil
    {
        public static void ExportToExcel(string filePath, List<Member> members)
        {
            try
            {
                XSSFWorkbook book = new XSSFWorkbook();
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("dt.TableName");

                //headers
                NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("编号");
                row.CreateCell(1).SetCellValue("姓名");
                row.CreateCell(2).SetCellValue("身份证号");
                row.CreateCell(3).SetCellValue("性别");
                row.CreateCell(4).SetCellValue("银行");
                row.CreateCell(5).SetCellValue("账号");
                row.CreateCell(6).SetCellValue("电话");
                row.CreateCell(7).SetCellValue("缴费");
                row.CreateCell(8).SetCellValue("奖金");
                row.CreateCell(9).SetCellValue("主管");
                row.CreateCell(10).SetCellValue("下属1");
                row.CreateCell(11).SetCellValue("下属2");
                row.CreateCell(12).SetCellValue("加入日期");
                row.Cells.ForEach(c => c.CSS("color:black;font-weight:bold;background-color:orange;border-type:thin"));

                for (int i = 0; i < members.Count; i++)
                {
                    NPOI.SS.UserModel.IRow row2 = sheet.CreateRow(i + 1);
                    row2.CreateCell(0).SetCellValue(Convert.ToString(members[i].MID));
                    row2.CreateCell(1).SetCellValue(Convert.ToString(members[i].Name));
                    row2.CreateCell(2).SetCellValue(Convert.ToString(members[i].IDNumber));
                    row2.CreateCell(3).SetCellValue(members[i].Gender == Gender.Female ? "女" : "男");
                    row2.CreateCell(4).SetCellValue(members[i].Bank);
                    row2.CreateCell(5).SetCellValue(members[i].Account);
                    row2.CreateCell(6).SetCellValue(members[i].Phone);
                    row2.CreateCell(7).SetCellValue(Convert.ToString(members[i].Fee));
                    row2.CreateCell(8).SetCellValue(Convert.ToString(members[i].Bonus));
                    row2.CreateCell(9).SetCellValue(Convert.ToString(members[i].Parent?.Name));

                    if (members[i].Children.Count > 0)
                        row2.CreateCell(10).SetCellValue(Convert.ToString(members[i].Children[0]?.Name));
                    else
                        row2.CreateCell(10).SetCellValue(string.Empty);

                    if (members[i].Children.Count > 1)
                        row2.CreateCell(11).SetCellValue(Convert.ToString(members[i].Children[1]?.Name));
                    else
                        row2.CreateCell(11).SetCellValue(string.Empty);

                    if (string.IsNullOrEmpty(members[i].JoinDate))
                        row2.CreateCell(12).SetCellValue("");
                    else
                        row2.CreateCell(12).SetCellValue(Convert.ToDateTime(members[i].JoinDate).ToString("yyyy-MM-dd"));

                    row2.Cells.ForEach(c => c.CSS("color:black;border-type:thin"));
                }


                // export to file
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    book.Write(ms);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                    book = null;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
