using System.Collections.Generic;
using System.Data;
using NPOI.SS.UserModel;

namespace DP.Excel
{
    public abstract class ExcelHelper
    {
        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.BLANK: //BLANK:
                    return null;
                case CellType.BOOLEAN: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.NUMERIC: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.STRING: //STRING:
                    return cell.StringCellValue;
                case CellType.ERROR: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.FORMULA: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="columnNameMap"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected string GetTableHeaderShowName(Dictionary<string, string> columnNameMap, string columnName)
        {
            string showName = columnName;
            if (columnNameMap.ContainsKey(columnName.ToLower()))
            {
                showName = columnNameMap[columnName.ToLower()];
            }
            else if (columnNameMap.ContainsKey(columnName))
            {
                showName = columnNameMap[columnName];
            }
            else
            {
                showName = columnName;
            }
            return showName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fileName"></param>
        public abstract void DataTableToExcel(DataTable dataTable, Dictionary<string, string> columnNameMap, string fileName, string sheetName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        public void DataTableToExcel(DataTable dataTable, string fileName, string sheetName)
        {
            DataTableToExcel(dataTable, new Dictionary<string, string>(), fileName, sheetName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public abstract DataTable ExcelToDataTable(string fileName, string sheetName);

    }
}