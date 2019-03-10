using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;


namespace AdaptivBot
{
    public static class ExcelUtils
    {
        public static bool SheetExists(Excel.Workbook workbook, string sheetName)
        {
            foreach (Excel.Worksheet ws in workbook.Sheets)
            {
                if (ws.Name == sheetName)
                {
                    return true;
                }
            }

            return false;
        }


        public static void WriteToCell(
            Excel.Range rng,
            string valueToWrite,
            bool bold = true)
        {
            rng.Value2 = valueToWrite;
            rng.Font.Bold = bold;
        }


        public static void WriteToCell(
            Excel.Range rng,
            double valueToWrite,
            bool bold = false,
            string numberFormat = "#,##0")
        {
            rng.Value2 = valueToWrite;
            rng.Font.Bold = bold;
            rng.NumberFormat = numberFormat;
        }


        // TODO: This function needs significant refactoring.
        public static void WriteOutputBlockToExcel(
            Excel.Application xlApp,
            Excel.Workbook workbook,
            string wsName,
            string[] titles,
            object[,] output,
            int rowOffset
        )
        {
            if (output.GetLength(0) > 0)
            {
                Excel.Worksheet ws = null;
                if (SheetExists(workbook, wsName))
                {
                    ws = xlApp.Sheets[wsName];
                }
                else
                {
                    ws =
                        xlApp.Sheets.Add(After: xlApp.Worksheets[xlApp.Worksheets.Count]);
                    ws.Name = wsName;
                }

                Excel.Range c1;
                Excel.Range c2;
                Excel.Range range;
                var titleOffset = 0;
                if (titles.Length > 0)
                {
                    titleOffset = 1;
                    Excel.Range baseCell = ws.Cells[1, 1];
                    c1 = baseCell.Offset[0, 0];
                    c2 = c1.Offset[0, titles.Length - 1];
                    range = ws.get_Range(c1, c2);
                    range.Value = titles;
                    range.Font.Bold = true;
                }

                c1 = ws.Cells[1 + titleOffset + rowOffset, 1];
                c2 = ws.Cells[1 + titleOffset + rowOffset + output.GetLength(0) - 1, output.GetLength(1)];
                range = ws.get_Range(c1, c2);
                range.Value = output;
                range.NumberFormat = "#,##0";
                ws.Columns["C:C"].NumberFormat = "d-mmm-yy";
                ws.Columns["M:M"].NumberFormat = "d-mmm-yy";

                range.Columns.AutoFit();

                foreach (Excel.Range column in range.Columns)
                {
                    if (column.ColumnWidth == 255)
                    {
                        column.ColumnWidth = 30;
                    }
                }

                ws.Activate();
                xlApp.ActiveWindow.DisplayGridlines = false;
            }
        }
    }
}
