using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ars.Commom.Tool.Extension
{
    public static class IEnumerableExtension
    {
        public static bool HasValue<T>(this IEnumerable<T>? ts) 
        {
            return ts?.Any() ?? false;
        }

        public static bool HasNotValue<T>(this IEnumerable<T>? ts)
        {
            return !ts.HasValue<T>();
        }

        public static DataSet ToDataSet<T>(this IEnumerable<T> list,string tableName = "") 
        {
            var itemtype = typeof(T);

            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            foreach (var prop in itemtype.GetProperties(BindingFlags.Public | BindingFlags.Instance)) 
            {
                dataTable.Columns.Add(prop.Name,prop.PropertyType);
            }

            object[] datas = null;
            foreach (var item in list) 
            {
                datas = new object[dataTable.Columns.Count];
                for (int i = 0; i < dataTable.Columns.Count; i++) 
                {
                    datas[i] = itemtype.GetProperty(dataTable.Columns[i].ColumnName)!.GetValue(item)!;
                }

                dataTable.Rows.Add(datas);
            }
            dataTable.TableName = tableName.IsNullOrEmpty() ? itemtype.Name : tableName;
            dataSet.Tables.Add(dataTable);

            return dataSet;
        }

        /// <summary>
        /// 舍弃重复的item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, K>(this IEnumerable<T> source, Func<T, K> predicate)
        {
            HashSet<K> sets = new HashSet<K>();
            foreach (var item in source)
            {
                if (sets.Add(predicate(item)))
                {
                    yield return item;
                }
            }
        }
    }
}
