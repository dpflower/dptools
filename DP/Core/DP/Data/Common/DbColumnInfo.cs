using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DP.Data.Common
{
    public class DbColumnInfo
    {
        #region 变量
        protected PropertyInfo _Property;
        protected string _PropertyName;
        protected string _ColumnName;
        protected string _ColumnDescription;
        protected Type _Type;
        protected bool _PrimaryKey;
        protected bool _IncrementPrimaryKey;
        protected bool _Ignore;
        protected MethodInfo _SetMethod;
        protected MethodInfo _GetMethod; 
        #endregion

        #region 属性
        /// <summary>
        /// Gets the property.
        /// </summary>
        public PropertyInfo Property
        {
            get { return _Property; }
        }
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName
        {
            get { return _PropertyName; }
        }
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName
        {
            get { return _ColumnName; }
        }
        /// <summary>
        /// Gets the column description.
        /// </summary>
        public string ColumnDescription
        {
            get { return _ColumnDescription; }
        }
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type
        {
            get { return _Type; }
        }
        /// <summary>
        /// Gets a value indicating whether [primary key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [primary key]; otherwise, <c>false</c>.
        /// </value>
        public bool PrimaryKey
        {
            get { return _PrimaryKey; }
        }
        /// <summary>
        /// Gets a value indicating whether [increment primary key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [increment primary key]; otherwise, <c>false</c>.
        /// </value>
        public bool IncrementPrimaryKey
        {
            get { return _IncrementPrimaryKey; }
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="DbColumnInfo"/> is ignore.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ignore; otherwise, <c>false</c>.
        /// </value>
        public bool Ignore
        {
            get { return _Ignore; }
        }
        /// <summary>
        /// Gets the set method.
        /// </summary>
        public MethodInfo SetMethod
        {
            get { return _SetMethod; }
        }
        /// <summary>
        /// Gets the get method.
        /// </summary>
        public MethodInfo GetMethod
        {
            get { return _GetMethod; }
        } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DbColumnInfo"/> class.
        /// </summary>
        /// <param name="prop">The prop.</param>
        public DbColumnInfo(PropertyInfo prop)
        {
            _Property = prop;
            _PropertyName = prop.Name;
            _Type = prop.PropertyType;
            _SetMethod = prop.GetSetMethod(false);
            _GetMethod = prop.GetGetMethod(false); 
        }
    }
}
