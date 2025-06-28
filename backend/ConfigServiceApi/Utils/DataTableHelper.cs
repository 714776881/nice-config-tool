using System;
using System.Collections.Generic;
using System.Data;

namespace ConfigServiceApi.Utils
{
    public class DataTableHelper
    {
        // 将 DataTable 转换为 List<Dictionary<string, string>>
        public static List<Dictionary<string, string>> ConvertToListOfDictionary(DataTable table)
        {
            var result = new List<Dictionary<string, string>>();

            foreach (DataRow row in table.Rows)
            {
                var rowDict = new Dictionary<string, string>();
                foreach (DataColumn column in table.Columns)
                {
                    string key = column.ColumnName;
                    string value = row[column].ToString();
                    rowDict.Add(key, value);
                }
                result.Add(rowDict);
            }

            return result;
        }

        // 将 List<Dictionary<string, string>> 转换为 DataTable
        public static DataTable ConvertToDataTable(List<Dictionary<string, string>> list)
        {
            var table = new DataTable();

            if (list == null || list.Count == 0)
                return table;

            // Create columns
            foreach (var key in list[0].Keys)
            {
                table.Columns.Add(key);
            }

            // Create rows
            foreach (var dict in list)
            {
                var row = table.NewRow();
                foreach (var key in dict.Keys)
                {
                    row[key] = dict[key];
                }
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
