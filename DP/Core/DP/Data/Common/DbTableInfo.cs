using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DP.Data.Common
{
    public class DbTableInfo
    {
        protected string _TableName;

        public string TableName
        {
            get { return _TableName; }
        }
        protected string _ClassName;

        public string ClassName
        {
            get { return _ClassName; }
        }
        protected string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
        }
        protected TableType _TableOrView;

        public TableType TableOrView
        {
            get { return _TableOrView; }
        }

        public DbTableInfo(Type type)
        {
            //TableAttribute attr = Attribute.GetCustomAttribute(type, typeof(TableAttribute), true) as TableAttribute;
            //if (attr == null)
            //{
            //    attr = new TableAttribute();
            //}
            
        }
        
    }
}
