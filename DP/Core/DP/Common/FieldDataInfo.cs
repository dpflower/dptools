using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DP.Common
{
    [Serializable]
    public class FieldDataInfo
    {
        public FieldDataInfo()
        {

        }


        string _fieldKey = string.Empty;

        string _fieldValue = string.Empty;

        Type _fieldType = Type.GetType("System.String");

        /// <summary>
        /// Gets or sets the field key.
        /// </summary>
        /// <value>
        /// The field key.
        /// </value>
        public string FieldKey
        {
            get { return _fieldKey; }
            set { _fieldKey = value; }
        }
        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public string FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }
        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public Type FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }

        /// <summary>
        /// Gets the row info.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        public static List<FieldDataInfo> GetRowInfo(DataTable dt, int rowIndex)
        {
            if (dt == null)
            {
                return null;
            }
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            if (dt.Rows.Count < rowIndex)
            {
                return null;
            }
            List<FieldDataInfo> list = new List<FieldDataInfo>();            
            FieldDataInfo field;
            foreach (DataColumn column in dt.Columns)
            {
                field = new FieldDataInfo();
                field.FieldKey = column.ColumnName;
                if (column.DataType == Type.GetType("System.DateTime"))
                {
                    field.FieldValue = DateTimeHelper.Format(dt.Rows[rowIndex][column.ColumnName].ToString(), "yyyy-MM-dd HH:mm:ss");
                }
                field.FieldType = column.DataType;
                list.Add(field);
            }
            return list;
        }

        /// <summary>
        /// Sets the field data info.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool SetFieldDataInfo(List<FieldDataInfo> list, string key, string value, Type type)
        {
            bool rel = true;
            try
            {
                FieldDataInfo field = list.Find(delegate(FieldDataInfo entity) { if (entity.FieldKey.ToLower() == key.ToLower()) return true; else return false; });
                if (field != null)
                {
                    field.FieldValue = value;
                    if (type != null)
                    {
                        field.FieldType = type;
                    }
                }
                else
                {
                    field.FieldKey = key;
                    field.FieldValue = value;
                    field.FieldType = type;
                    list.Add(field);
                }
            }
            catch (Exception ex)
            {
                rel = false;
            }
            return rel;
        }



    }
}
