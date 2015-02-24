using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.Common;
using System.Configuration;

namespace DP.Data.OracleClient
{
    public class OracleDbTableInfo : DbTableInfo
    {
        protected OracleVerson _OracleVerson;

        public OracleVerson OracleVerson
        {
            get { return _OracleVerson; }
        }

        public OracleDbTableInfo(Type type)
            : base(type)
        {
            OracleTableAttribute attr = Attribute.GetCustomAttribute(type, typeof(OracleTableAttribute), true) as OracleTableAttribute;
            if (attr == null)
            {
                attr = new OracleTableAttribute();
            }
            _OracleVerson = attr.OracleVerson;
            _ClassName = type.Name;
            _TableName = String.IsNullOrEmpty(attr.TableName) ? type.Name : attr.TableName;
            try
            {
                _ConnectionString = ConfigurationManager.ConnectionStrings[attr.ConnectionStringKey].ConnectionString;
            }
            catch
            {

            }
            _TableOrView = attr.TableOrView;


        }
    }
}
