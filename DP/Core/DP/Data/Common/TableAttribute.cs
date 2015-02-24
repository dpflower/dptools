using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Data.Common
{
    public class TableAttribute : Attribute
    {
        public TableAttribute() 
        {
 
        }

        string _tableName = "";
        string _connectionStringKey = "";
        TableType _tableOrView = TableType.Table;


        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public TableType TableOrView
        {
            get { return _tableOrView; }
            set { _tableOrView = value; }
        }

        public string ConnectionStringKey
        {
            get { return _connectionStringKey; }
            set { _connectionStringKey = value; }
        }



    }
}
