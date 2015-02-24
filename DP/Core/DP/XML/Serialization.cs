using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using DP.Common;

namespace DP.XML
{
    public static class Serialization
    {
        /// <summary>
        /// 对象列表序列化成XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">对象列表</param>
        /// <returns>的XML字符串</returns>
        public static string Serialize<T>(List<T> list)
        {
            XmlDocument result = new XmlDocument();
            result.LoadXml("<Root></Root>");
            foreach (T obj in list)
            {
                XmlElement Item = result.CreateElement("Item");
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj, null) != null)
                    {
                        XmlElement element = result.CreateElement(property.Name);
                        element.SetAttribute("Type", property.PropertyType.Name);
                        element.InnerText = property.GetValue(obj, null).ToString();
                        Item.AppendChild(element);
                    }
                }
                result.DocumentElement.AppendChild(Item);
            }
            return result.InnerXml;
        }

        /// <summary>
        /// XML字符串反序列化成对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="XMLString">需要反序列化的XML字符串</param>
        /// <returns>对象列表</returns>
        public static List<T> Deserialize<T>(string XMLString)
        {
            List<T> result = new List<T>();
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(XMLString);
            foreach (XmlNode ItemNode in XmlDoc.GetElementsByTagName("Root").Item(0).ChildNodes)
            {
                T item = Activator.CreateInstance<T>();
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (XmlNode propertyNode in ItemNode.ChildNodes)
                {
                    string name = propertyNode.Name;
                    string type = propertyNode.Attributes["Type"].Value;
                    string value = propertyNode.InnerXml;
                    foreach (PropertyInfo property in properties)
                    {
                        if (name == property.Name)
                        {
                            property.SetValue(item, ReflectionHelper.ChangeType(value, property.PropertyType), null);
                        }
                    }
                }
                result.Add(item);
            }
            return result;
        }

       
    }
}
