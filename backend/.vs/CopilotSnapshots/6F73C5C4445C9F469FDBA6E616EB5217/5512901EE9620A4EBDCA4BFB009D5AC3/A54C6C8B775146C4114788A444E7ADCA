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
    }
}
