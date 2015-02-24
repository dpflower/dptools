using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DP.Data.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class DbColumnSchema
    {
        #region 变量
        /// <summary>
        /// 
        /// </summary>
        protected PropertyInfo _Property;
        /// <summary>
        /// 
        /// </summary>
        protected string _PropertyName;
        /// <summary>
        /// 
        /// </summary>
        protected string _ColumnName;
        /// <summary>
        /// 
        /// </summary>
        protected string _ColumnDescription;
        /// <summary>
        /// 
        /// </summary>
        protected Type _Type;
        /// <summary>
        /// 
        /// </summary>
        protected bool _isPrimaryKey;
        /// <summary>
        /// 
        /// </summary>
        protected bool _isDbGenerated;
        /// <summary>
        /// 
        /// </summary>
        protected bool _isIgnore;
        /// <summary>
        /// 
        /// </summary>
        protected MethodInfo _SetMethod;
        /// <summary>
        /// 
        /// </summary>
        protected MethodInfo _GetMethod; 
        #endregion

        #region 属性
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <remarks></remarks>
        public PropertyInfo Property
        {
            get { return _Property; }
        }
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <remarks></remarks>
        public string PropertyName
        {
            get { return _PropertyName; }
        }
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        /// <remarks></remarks>
        public string ColumnName
        {
            get { return _ColumnName; }
        }
        /// <summary>
        /// Gets the column description.
        /// </summary>
        /// <remarks></remarks>
        public string ColumnDescription
        {
            get { return _ColumnDescription; }
        }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <remarks></remarks>
        public Type Type
        {
            get { return _Type; }
        }
        /// <summary>
        /// Gets a value indicating whether [primary key].
        /// </summary>
        /// <value><c>true</c> if [primary key]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
        }
        /// <summary>
        /// Gets a value indicating whether [increment primary key].
        /// </summary>
        /// <value><c>true</c> if [increment primary key]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsDbGenerated
        {
            get { return _isDbGenerated; }
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="DbColumnInfo"/> is ignore.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsIgnore
        {
            get { return _isIgnore; }
        }
        /// <summary>
        /// Gets the set method.
        /// </summary>
        /// <remarks></remarks>
        public MethodInfo SetMethod
        {
            get { return _SetMethod; }
        }
        /// <summary>
        /// Gets the get method.
        /// </summary>
        /// <remarks></remarks>
        public MethodInfo GetMethod
        {
            get { return _GetMethod; }
        } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DbColumnInfo"/> class.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <remarks></remarks>
        public DbColumnSchema(PropertyInfo prop)
        {
            _Property = prop;
            _PropertyName = prop.Name;
            _Type = prop.PropertyType;
            _SetMethod = prop.GetSetMethod(false);
            _GetMethod = prop.GetGetMethod(false); 
        }
    }
}
