using HoPoSim.IO.Interfaces;
using NetOffice.ExcelApi.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Reflection;
using Excel = NetOffice.ExcelApi;

namespace HoPoSim.IO
{

    [Export(typeof(IReportingService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReportingService : IReportingService
    {

        public void ExportToExcel<T>(IEnumerable<T> query, string file)
        {



            //var query = from o in db.Orders
            //            join od in db.Order_Details on o.OrderID equals od.OrderID
            //            where o.OrderID == orderid
            //            select new
            //            {
            //                OrderID = o.OrderID,
            //                OrderDate = o.OrderDate,
            //                ShipAddress = o.ShipAddress,
            //                Quantity = od.Quantity,
            //                UnitPrice = od.UnitPrice
            //            };


            DataTable dt = LINQToDataTable(query);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ExportExcel(ds, file);


        }

        private void ExportExcel(DataSet dataset, string file)
        {
            //Create excel application
            var application = new Excel.Application();

            //Add workbook
            application.Workbooks.Add();

            application.DisplayAlerts = false;

            //Get active worksheet
            var sheet = (Excel.Worksheet)application.ActiveSheet;
            sheet.Name = "Sample excel....";

            foreach (DataTable dataTable in dataset.Tables)
            {
                //Get all data into an array
                var tempArray = new object[dataTable.Rows.Count, dataTable.Columns.Count];
                for (var r = 0; r < dataTable.Rows.Count; r++)
                {
                    for (var c = 0; c < dataTable.Columns.Count; c++)
                        tempArray[r, c] = dataTable.Rows[r][c];
                }

                //Get column names into an array
                var tempHeadingArray = new object[dataTable.Columns.Count];
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    tempHeadingArray[i] = dataTable.Columns[i].ColumnName;
                }

                //Create style used for displaying column names
                var style = application.ActiveWorkbook.Styles.Add("NewStyle");
                style.Font.Name = "Verdana";
                style.Font.Size = 10;
                //style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                style.Font.Bold = true;

                AddColumnNames(sheet, tempHeadingArray);

                AddExcelHeadingText(sheet);

                AddDataRows(sheet, dataTable, tempArray);
            }
            sheet.Columns.AutoFit();

            application.ActiveWorkbook.SaveAs(file, Missing.Value, Missing.Value, Missing.Value, false,
                                              false, XlSaveAsAccessMode.xlExclusive);

            //CleanUp
            application.ActiveWorkbook.Close();
            application.Quit();
            application.Dispose();
        }


        private static void AddDataRows(Excel.Worksheet sheet, DataTable datatable, object[,] tempArray)
        {
            var range = sheet.Range(sheet.Cells[4, 1],
                            sheet.Cells[(datatable.Rows.Count), (datatable.Columns.Count)]);

            range.Value = tempArray;
        }

        private static void AddColumnNames(Excel.Worksheet sheet, object[] tempHeadingArray)
        {
            var columnNameRange = sheet.get_Range(sheet.Cells[3, 3], sheet.Cells[3, tempHeadingArray.Length + 2]);
            columnNameRange.Style = "NewStyle";
            columnNameRange.Value = tempHeadingArray;
            columnNameRange.UseStandardWidth = true;
        }

        private static void AddExcelHeadingText(Excel.Worksheet sheet)
        {
            //Apply styling to heading text
            sheet.Cells[1, 1].Value = "Excel heading text";
            sheet.Cells[1, 1].Font.Name = "Verdana";
            sheet.Cells[1, 1].Font.Italic = true;
            sheet.Cells[1, 1].Font.Underline = true;
            sheet.Cells[1, 1].Font.Size = 14;
            //sheet.Cells[1, 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkRed);
        }


        private DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();


            // column names
            PropertyInfo[] oProps = null;


            if (varlist == null) return dtReturn;


            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;


                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }


                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }


                DataRow dr = dtReturn.NewRow();


                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }


                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

    }
}
