using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Collections.Concurrent;

namespace DP.Data.Common.Emit
{
    public static class FuncManager
    {
        private delegate void DataRecordToProperty(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex);

        #region DataRecord MethodInfo
        private static readonly MethodInfo DataRecord_GetOrdinal = typeof(IDataRecord).GetMethod("GetOrdinal");
        private static readonly MethodInfo DataRecord_GetName = typeof(IDataRecord).GetMethod("GetName");
        private static readonly MethodInfo DataRecord_GetValue = typeof(IDataRecord).GetMethod("GetValue");
        private static readonly MethodInfo DataRecord_IsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");

        private static readonly MethodInfo DataRecord_GetBoolean = typeof(IDataRecord).GetMethod("GetBoolean");
        private static readonly MethodInfo DataRecord_GetDateTime = typeof(IDataRecord).GetMethod("GetDateTime");
        private static readonly MethodInfo DataRecord_GetDecimal = typeof(IDataRecord).GetMethod("GetDecimal");
        private static readonly MethodInfo DataRecord_GetDouble = typeof(IDataRecord).GetMethod("GetDouble");
        private static readonly MethodInfo DataRecord_GetFloat = typeof(IDataRecord).GetMethod("GetFloat");
        private static readonly MethodInfo DataRecord_GetInt16 = typeof(IDataRecord).GetMethod("GetInt16");
        private static readonly MethodInfo DataRecord_GetInt32 = typeof(IDataRecord).GetMethod("GetInt32");
        private static readonly MethodInfo DataRecord_GetInt64 = typeof(IDataRecord).GetMethod("GetInt64");

        private static readonly MethodInfo DataRecord_GetGuid = typeof(IDataRecord).GetMethod("GetGuid");
        private static readonly MethodInfo DataRecord_GetString = typeof(IDataRecord).GetMethod("GetString");
        private static readonly MethodInfo DataRecord_GetChar = typeof(IDataRecord).GetMethod("GetChar");
        private static readonly MethodInfo DataRecord_GetByte = typeof(IDataRecord).GetMethod("GetByte");
        private static readonly MethodInfo Convert_IsDBNull = typeof(Convert).GetMethod("IsDBNull");

        private static readonly PropertyInfo DataRecord_FieldCount = typeof(IDataRecord).GetProperty("FieldCount");
        #endregion

        #region String MethodInfo
        private static readonly MethodInfo String_Equals_StringComparison = typeof(String).GetMethod("Equals", new Type[] { typeof(string), typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo String_Concat2 = typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
        private static readonly MethodInfo String_Concat3 = typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) });
        
        #endregion

        private static ConcurrentDictionary<Type, Func<IDataRecord, object>> funcDict = new ConcurrentDictionary<Type, Func<IDataRecord, object>>();
        private static ConcurrentDictionary<Type, Func<object, DbParameter[]>> funcDbparameterDict = new ConcurrentDictionary<Type, Func<object, DbParameter[]>>();
        private static Dictionary<Type, DataRecordToProperty> dataRecordDelegateDict = new Dictionary<Type, DataRecordToProperty>();

        static FuncManager()
        {
            dataRecordDelegateDict.Add(typeof(byte), DataRecordToPropertyByte);
            dataRecordDelegateDict.Add(typeof(byte?), DataRecordToPropertyNullableByte);
            dataRecordDelegateDict.Add(typeof(Int16), DataRecordToPropertyInt16);
            dataRecordDelegateDict.Add(typeof(Int16?), DataRecordToPropertyNullableInt16);
            dataRecordDelegateDict.Add(typeof(Int32), DataRecordToPropertyInt32);
            dataRecordDelegateDict.Add(typeof(Int32?), DataRecordToPropertyNullableInt32);
            dataRecordDelegateDict.Add(typeof(Int64), DataRecordToPropertyInt64);
            dataRecordDelegateDict.Add(typeof(Int64?), DataRecordToPropertyNullableInt64);
            dataRecordDelegateDict.Add(typeof(float), DataRecordToPropertyFloat);
            dataRecordDelegateDict.Add(typeof(float?), DataRecordToPropertyNullableFloat);
            dataRecordDelegateDict.Add(typeof(Double), DataRecordToPropertyDouble);
            dataRecordDelegateDict.Add(typeof(Double?), DataRecordToPropertyNullableDouble);
            dataRecordDelegateDict.Add(typeof(Decimal), DataRecordToPropertyDecimal);
            dataRecordDelegateDict.Add(typeof(Decimal?), DataRecordToPropertyNullableDecimal);
            dataRecordDelegateDict.Add(typeof(DateTime), DataRecordToPropertyDateTime);
            dataRecordDelegateDict.Add(typeof(DateTime?), DataRecordToPropertyNullableDateTime);
            dataRecordDelegateDict.Add(typeof(bool), DataRecordToPropertyBoolean);
            dataRecordDelegateDict.Add(typeof(bool?), DataRecordToPropertyNullableBoolean);
            dataRecordDelegateDict.Add(typeof(string), DataRecordToPropertyString);
            dataRecordDelegateDict.Add(typeof(object), DataRecordToPropertyObject);
        }

        /// <summary>
        /// 返回 IDataRecord 对象 转  Type类型对象 的 Func<IDataRecord, object>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<IDataRecord, object> IDataRecordToEntityFuncFactory(Type type)
        {
            Func<IDataRecord, object> func;
            if (funcDict.TryGetValue(type, out func))
            {
                return func;
            }
            func = IDataRecordToEntityByEmitBuilder(type);
            funcDict.TryAdd(type, func);
            return func;
            //if (funcDict.ContainsKey(type))
            //{
            //    return funcDict[type];
            //}
            //func = IDataRecordToEntityByEmitBuilder(type);
            //funcDict[type] = func;
            //return func;
        }

        /// <summary>
        /// 返回 IDataRecord 对象 转  Type类型对象 的 Func<IDataRecord, object>委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<IDataRecord, object> IDataRecordToEntityFuncFactory<T>()
        {
            Type type = typeof(T);
            return IDataRecordToEntityFuncFactory(type);
        }

        /// <summary>
        /// 返回 IDataRecord 对象 转  Type类型对象 的 Func<IDataRecord, object>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Func<IDataRecord, object> IDataRecordToEntityByEmitBuilder(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            //int propCount = properties.Length;
            DataRecordToProperty dataRecord;

            DynamicMethod dm = new DynamicMethod("FuncIDataRecordToEntity" + type.Namespace + "" + type.Name, type, new Type[] { typeof(IDataRecord) }, type);
            ILGenerator il = dm.GetILGenerator();

            LocalBuilder bEquals = il.DeclareLocal(typeof(bool));
            LocalBuilder fieldName = il.DeclareLocal(typeof(string));
            LocalBuilder index = il.DeclareLocal(typeof(Int32));
            LocalBuilder fieldCount = il.DeclareLocal(typeof(Int32));
            LocalBuilder propertyName = il.DeclareLocal(typeof(String));

            //T obj;
            LocalBuilder obj = il.DeclareLocal(type);

            //obj = new T();
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, obj);

            //fieldCount = IDataReader.FieldCount;
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DataRecord_FieldCount.GetGetMethod());
            il.Emit(OpCodes.Stloc_S, fieldCount);

            foreach (PropertyInfo propertyInfo in properties)
            {
                il.Emit(OpCodes.Ldstr, propertyInfo.Name);
                il.Emit(OpCodes.Stloc_S, propertyName);

                Label exit = il.DefineLabel();
                Label loop = il.DefineLabel();
                Label first = il.DefineLabel();
                Label next = il.DefineLabel();


                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Stloc_S, index);
                il.Emit(OpCodes.Br_S, first);

                il.MarkLabel(loop);

                #region For 循环体  try/catch(Exception ex){ do }
                //try
                Label tryLabel = il.BeginExceptionBlock();


                //fieldName = dataReader.GetName(index);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, index);
                il.Emit(OpCodes.Callvirt, DataRecord_GetName);
                il.Emit(OpCodes.Stloc_S, fieldName);

                //bEquals = string.Equals(fieldName, propertyName, StringComparison.OrdinalIgnoreCase);
                il.Emit(OpCodes.Ldloc_S, fieldName);
                il.Emit(OpCodes.Ldloc_S, propertyName);
                il.Emit(OpCodes.Ldc_I4_5);
                il.Emit(OpCodes.Call, String_Equals_StringComparison);
                il.Emit(OpCodes.Stloc_S, bEquals);                //
                il.Emit(OpCodes.Ldloc_S, bEquals);
                il.Emit(OpCodes.Brfalse_S, next);

                //obj.Property = dataReader.GetValue(i);
                #region 给对象属性赋值
                if (dataRecordDelegateDict.ContainsKey(propertyInfo.PropertyType))
                {
                    dataRecord = dataRecordDelegateDict[propertyInfo.PropertyType];
                }
                else
                {
                    dataRecord = dataRecordDelegateDict[typeof(object)];
                }
                dataRecord(obj, il, propertyInfo, index);
                #endregion

                //catch(Exception ex){ throw new Exception(ex.Message+"|PropertyInfo_Name:" +"", ex);}
                il.BeginCatchBlock(typeof(Exception));
                LocalBuilder ex = il.DeclareLocal(typeof(Exception));
                il.Emit(OpCodes.Stloc_S, ex);
                il.Emit(OpCodes.Ldloc_S, ex);
                il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
                il.Emit(OpCodes.Ldstr, "\r\n Wrong property name is : ");
                il.Emit(OpCodes.Ldstr, propertyInfo.Name);
                il.Emit(OpCodes.Call, String_Concat3);
                il.Emit(OpCodes.Ldloc_S, ex);
                il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[] { typeof(string), typeof(Exception) }));
                il.Emit(OpCodes.Throw);
                il.EndExceptionBlock();

                //break;
                il.Emit(OpCodes.Br_S, exit);
                #endregion

                il.MarkLabel(next);
                il.Emit(OpCodes.Ldloc_S, index);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc_S, index);
                il.MarkLabel(first);
                il.Emit(OpCodes.Ldloc_S, index);
                il.Emit(OpCodes.Ldloc_S, fieldCount);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Stloc_S, bEquals);
                il.Emit(OpCodes.Ldloc_S, bEquals);
                il.Emit(OpCodes.Brtrue_S, loop);
                il.MarkLabel(exit);
            }


            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ret);

            return (Func<IDataRecord, object>)dm.CreateDelegate(typeof(Func<IDataRecord, object>));
        }

        /// <summary>
        /// 返回 IDataRecord 对象 转  Type类型对象 的 Func<IDataRecord, object>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<IDataRecord, object> IDataRecordToEntityFuncFactory(Type type, List<DbColumnSchema> dbColumnSchemaList)
        {
            Func<IDataRecord, object> func;
            if (funcDict.TryGetValue(type, out func))
            {
                return func;
            }
            func = IDataRecordToEntityByEmitBuilder(type);
            funcDict.TryAdd(type, func);
            return func;
            //if (funcDict.ContainsKey(type))
            //{
            //    return funcDict[type];
            //}
            //Func<IDataRecord, object> func = IDataRecordToEntityByEmitBuilder(type, dbColumnSchemaList);
            //funcDict[type] = func;
            //return func;
        }

        /// <summary>
        /// 返回 IDataRecord 对象 转  Type类型对象 的 Func<IDataRecord, object>委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<IDataRecord, object> IDataRecordToEntityFuncFactory<T>(List<DbColumnSchema> dbColumnSchemaList)
        {
            Type type = typeof(T);
            return IDataRecordToEntityFuncFactory(type, dbColumnSchemaList);
        }

        /// <summary>
        /// 返回 IDataRecord 对象 转  Type类型对象 的 Func<IDataRecord, object>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Func<IDataRecord, object> IDataRecordToEntityByEmitBuilder(Type type, List<DbColumnSchema> dbColumnSchemaList)
        {
            PropertyInfo[] properties = type.GetProperties();
            //int propCount = properties.Length;
            DataRecordToProperty dataRecord;

            DynamicMethod dm = new DynamicMethod("FuncIDataRecordToEntity" + type.Namespace + "" + type.Name, type, new Type[] { typeof(IDataRecord) }, type);
            ILGenerator il = dm.GetILGenerator();

            LocalBuilder bEquals = il.DeclareLocal(typeof(bool));
            LocalBuilder fieldName = il.DeclareLocal(typeof(string));
            LocalBuilder index = il.DeclareLocal(typeof(Int32));
            LocalBuilder fieldCount = il.DeclareLocal(typeof(Int32));
            LocalBuilder propertyName = il.DeclareLocal(typeof(String));

            //T obj;
            LocalBuilder obj = il.DeclareLocal(type);

            //obj = new T();
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, obj);

            //fieldCount = IDataReader.FieldCount;
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DataRecord_FieldCount.GetGetMethod());
            il.Emit(OpCodes.Stloc_S, fieldCount);

            foreach (DbColumnSchema dbColumnSchema in dbColumnSchemaList)
            {
                il.Emit(OpCodes.Ldstr, dbColumnSchema.ColumnName);
                il.Emit(OpCodes.Stloc_S, propertyName);

                Label exit = il.DefineLabel();
                Label loop = il.DefineLabel();
                Label first = il.DefineLabel();
                Label next = il.DefineLabel();


                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Stloc_S, index);
                il.Emit(OpCodes.Br_S, first);

                il.MarkLabel(loop);

                #region For 循环体  try/catch(Exception ex){ do }
                //try
                Label tryLabel = il.BeginExceptionBlock();


                //fieldName = dataReader.GetName(index);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, index);
                il.Emit(OpCodes.Callvirt, DataRecord_GetName);
                il.Emit(OpCodes.Stloc_S, fieldName);

                //bEquals = string.Equals(fieldName, propertyName, StringComparison.OrdinalIgnoreCase);
                il.Emit(OpCodes.Ldloc_S, fieldName);
                il.Emit(OpCodes.Ldloc_S, propertyName);
                il.Emit(OpCodes.Ldc_I4_5);
                il.Emit(OpCodes.Call, String_Equals_StringComparison);
                il.Emit(OpCodes.Stloc_S, bEquals);                //
                il.Emit(OpCodes.Ldloc_S, bEquals);
                il.Emit(OpCodes.Brfalse_S, next);

                //obj.Property = dataReader.GetValue(i);
                #region 给对象属性赋值
                if (dataRecordDelegateDict.ContainsKey(dbColumnSchema.Property.PropertyType))
                {
                    dataRecord = dataRecordDelegateDict[dbColumnSchema.Property.PropertyType];
                }
                else
                {
                    dataRecord = dataRecordDelegateDict[typeof(object)];
                }
                dataRecord(obj, il, dbColumnSchema.Property, index);
                #endregion

                //catch(Exception ex){ throw new Exception(ex.Message+"|PropertyInfo_Name:" +"", ex);}
                il.BeginCatchBlock(typeof(Exception));
                LocalBuilder ex = il.DeclareLocal(typeof(Exception));
                il.Emit(OpCodes.Stloc_S, ex);
                il.Emit(OpCodes.Ldloc_S, ex);
                il.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
                il.Emit(OpCodes.Ldstr, "\r\n Wrong property name is : ");
                il.Emit(OpCodes.Ldstr, dbColumnSchema.ColumnName);
                il.Emit(OpCodes.Call, String_Concat3);
                il.Emit(OpCodes.Ldloc_S, ex);
                il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[] { typeof(string), typeof(Exception) }));
                il.Emit(OpCodes.Throw);
                il.EndExceptionBlock();

                //break;
                il.Emit(OpCodes.Br_S, exit);
                #endregion

                il.MarkLabel(next);
                il.Emit(OpCodes.Ldloc_S, index);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc_S, index);
                il.MarkLabel(first);
                il.Emit(OpCodes.Ldloc_S, index);
                il.Emit(OpCodes.Ldloc_S, fieldCount);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Stloc_S, bEquals);
                il.Emit(OpCodes.Ldloc_S, bEquals);
                il.Emit(OpCodes.Brtrue_S, loop);
                il.MarkLabel(exit);
            }


            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ret);

            return (Func<IDataRecord, object>)dm.CreateDelegate(typeof(Func<IDataRecord, object>));
        }

        #region 给对象属性赋值 DataRecordToProperty
        private static void DataRecordToPropertyByte(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetByte);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableByte(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //LocalBuilder loc0 = il.DeclareLocal(typeof(Int16?));
            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetByte);
            il.Emit(OpCodes.Newobj, typeof(byte?).GetConstructor(new Type[] { typeof(byte) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.Emit(OpCodes.Br_S, label);

            il.MarkLabel(labelNull);

            //il.Emit(OpCodes.Ldloc_S, loc0);
            //il.Emit(OpCodes.Initobj, typeof(Int16?));

            //il.Emit(OpCodes.Ldloc_S, obj);
            //il.Emit(OpCodes.Ldloc_S, loc0);
            //il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            //il.MarkLabel(label);
        }

        private static void DataRecordToPropertyInt16(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt16);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableInt16(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //LocalBuilder loc0 = il.DeclareLocal(typeof(Int16?));
            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt16);
            il.Emit(OpCodes.Newobj, typeof(Int16?).GetConstructor(new Type[] { typeof(Int16) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.Emit(OpCodes.Br_S, label);

            il.MarkLabel(labelNull);

            //il.Emit(OpCodes.Ldloc_S, loc0);
            //il.Emit(OpCodes.Initobj, typeof(Int16?));

            //il.Emit(OpCodes.Ldloc_S, obj);
            //il.Emit(OpCodes.Ldloc_S, loc0);
            //il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            //il.MarkLabel(label);
        }

        private static void DataRecordToPropertyInt32(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableInt32(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {

            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
            il.Emit(OpCodes.Newobj, typeof(Int32?).GetConstructor(new Type[] { typeof(Int32) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);

        }

        private static void DataRecordToPropertyInt64(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableInt64(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
            il.Emit(OpCodes.Newobj, typeof(Int64?).GetConstructor(new Type[] { typeof(Int64) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);
        }

        private static void DataRecordToPropertyFloat(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetFloat);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableFloat(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetFloat);
            il.Emit(OpCodes.Newobj, typeof(float?).GetConstructor(new Type[] { typeof(float) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);
        }

        private static void DataRecordToPropertyDouble(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetDouble);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableDouble(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetDouble);
            il.Emit(OpCodes.Newobj, typeof(double?).GetConstructor(new Type[] { typeof(double) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);
        }

        private static void DataRecordToPropertyDecimal(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableDecimal(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            Label labelNull = il.DefineLabel();
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
            il.Emit(OpCodes.Newobj, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);

        }

        private static void DataRecordToPropertyDateTime(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableDateTime(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            Label labelNull = il.DefineLabel();
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
            il.Emit(OpCodes.Newobj, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);
        }

        private static void DataRecordToPropertyBoolean(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);
            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetBoolean);
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.MarkLabel(label);
        }
        private static void DataRecordToPropertyNullableBoolean(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetBoolean);
            il.Emit(OpCodes.Newobj, typeof(bool?).GetConstructor(new Type[] { typeof(bool) }));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);
        }

        private static void DataRecordToPropertyString(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            //Label label = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetValue);
            il.Emit(OpCodes.Callvirt, typeof(Object).GetMethod("ToString"));
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            //il.MarkLabel(label);
        }

        private static void DataRecordToPropertyObject(LocalBuilder obj, ILGenerator il, PropertyInfo propertyInfo, LocalBuilder colIndex)
        {
            Label labelNull = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Ldc_I4_M1);
            //il.Emit(OpCodes.Beq_S, label);


            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            il.Emit(OpCodes.Brtrue_S, labelNull);

            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_S, colIndex);
            il.Emit(OpCodes.Callvirt, DataRecord_GetValue);
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Isinst, propertyInfo.PropertyType);
                //il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
            }
            il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());


            //Label common = il.DefineLabel();
            //il.Emit(OpCodes.Ldloc_S, obj);
            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Callvirt, DataRecord_GetValue);
            //il.Emit(OpCodes.Dup);
            //il.Emit(OpCodes.Call, Convert_IsDBNull);
            //il.Emit(OpCodes.Brfalse_S, common);
            //il.Emit(OpCodes.Pop);
            //il.Emit(OpCodes.Ldnull);
            //il.MarkLabel(common);
            //il.Emit(OpCodes.Unbox_Any, propertyInfo.DeclaringType);
            //il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            //il.Emit(OpCodes.Ldloc_S, obj);

            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldloc_S, colIndex);
            //il.Emit(OpCodes.Callvirt, DataRecord_GetString);
            //il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

            il.MarkLabel(labelNull);
        }
        #endregion



        /// <summary>
        /// 返回 Object 对象 转 DbParameter数组的 Func<object, DbParameter[]>委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<object, DbParameter[]> EntityToDbParameterFuncFactory<T>()
        {
            Type type = typeof(T);
            return EntityToDbParameterFuncFactory(type);
        }

        /// <summary>
        /// 返回 Object 对象 转 DbParameter数组的 Func<object, DbParameter[]>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<object, DbParameter[]> EntityToDbParameterFuncFactory(Type type)
        {
            Func<object, DbParameter[]> func;
            if (funcDbparameterDict.TryGetValue(type, out func))
            {
                return func;
            }
            func = EntityToDbParameterByEmitBuilder(type);
            funcDbparameterDict.TryAdd(type, func);
            return func;

            //if (funcDbparameterDict.ContainsKey(type))
            //{
            //    return funcDbparameterDict[type];
            //}
            //Func<object, DbParameter[]> func = EntityToDbParameterByEmitBuilder(type);
            //funcDbparameterDict[type] = func;
            //return func;
        }

        /// <summary>
        ///  返回 Object 对象 转 DbParameter数组的 Func<object, DbParameter[]>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Func<object, DbParameter[]> EntityToDbParameterByEmitBuilder(Type type)
        {
            Type relListType = typeof(DbParameter[]);
            Type relType = typeof(System.Data.SqlClient.SqlParameter);
            Type inputType = type;
            PropertyInfo[] properties = inputType.GetProperties();
            int count = properties.Length;

            DynamicMethod dm = new DynamicMethod("Func_GetDbParameter_" + inputType.Namespace + "." + inputType.Name, relListType, new Type[] { typeof(object) }, inputType);
            ILGenerator il = dm.GetILGenerator();

            //T obj;
            LocalBuilder obj = il.DeclareLocal(relListType);

            //obj = new T();
            il.Emit(OpCodes.Ldc_I4_S, count);
            il.Emit(OpCodes.Newarr, relType);
            il.Emit(OpCodes.Stloc_S, obj);

            for (int i = 0; i < count; i++)
            {
                il.Emit(OpCodes.Ldloc_S, obj);
                il.Emit(OpCodes.Ldc_I4_S, i);
                il.Emit(OpCodes.Ldstr, "@" + properties[i].Name);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, properties[i].GetGetMethod());
                if (properties[i].PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, properties[i].PropertyType);
                }
                il.Emit(OpCodes.Newobj, relType.GetConstructor(new Type[] { typeof(string), typeof(object) }));
                il.Emit(OpCodes.Stelem_Ref);
            }


            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ret);
            return (Func<object, DbParameter[]>)dm.CreateDelegate(typeof(Func<object, DbParameter[]>));
        }

        /// <summary>
        /// 返回 Object 对象 转 DbParameter数组的 Func<object, DbParameter[]>委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<object, DbParameter[]> EntityToDbParameterFuncFactory<T>(List<DbColumnSchema> dbColumnSchemaList)
        {
            Type type = typeof(T);
            return EntityToDbParameterFuncFactory(type, dbColumnSchemaList);
        }

        /// <summary>
        /// 返回 Object 对象 转 DbParameter数组的 Func<object, DbParameter[]>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<object, DbParameter[]> EntityToDbParameterFuncFactory(Type type, List<DbColumnSchema> dbColumnSchemaList)
        {
            Func<object, DbParameter[]> func;
            if (funcDbparameterDict.TryGetValue(type, out func))
            {
                return func;
            }
            func = EntityToDbParameterByEmitBuilder(type);
            funcDbparameterDict.TryAdd(type, func);
            return func;

            //if (funcDbparameterDict.ContainsKey(type))
            //{
            //    return funcDbparameterDict[type];
            //}
            //Func<object, DbParameter[]> func = EntityToDbParameterByEmitBuilder(type, dbColumnSchemaList);
            //funcDbparameterDict[type] = func;
            //return func;
        }

        /// <summary>
        ///  返回 Object 对象 转 DbParameter数组的 Func<object, DbParameter[]>委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Func<object, DbParameter[]> EntityToDbParameterByEmitBuilder(Type type, List<DbColumnSchema> dbColumnSchemaList)
        {
            Type relListType = typeof(DbParameter[]);
            Type relType = typeof(System.Data.SqlClient.SqlParameter);
            Type inputType = type;
            //PropertyInfo[] properties = inputType.GetProperties();
            //int count = properties.Length;
            int count = dbColumnSchemaList.Count;

            DynamicMethod dm = new DynamicMethod("Func_GetDbParameter_" + inputType.Namespace + "." + inputType.Name, relListType, new Type[] { typeof(object) }, inputType);
            ILGenerator il = dm.GetILGenerator();

            //T obj;
            LocalBuilder obj = il.DeclareLocal(relListType);

            //obj = new T();
            il.Emit(OpCodes.Ldc_I4_S, count);
            il.Emit(OpCodes.Newarr, relType);
            il.Emit(OpCodes.Stloc_S, obj);

            for (int i = 0; i < count; i++)
            {
                il.Emit(OpCodes.Ldloc_S, obj);
                il.Emit(OpCodes.Ldc_I4_S, i);
                il.Emit(OpCodes.Ldstr, "@" + dbColumnSchemaList[i].ColumnName);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, dbColumnSchemaList[i].Property.GetGetMethod());
                if (dbColumnSchemaList[i].Property.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, dbColumnSchemaList[i].Property.PropertyType);
                }
                il.Emit(OpCodes.Newobj, relType.GetConstructor(new Type[] { typeof(string), typeof(object) }));
                il.Emit(OpCodes.Stelem_Ref);
            }


            il.Emit(OpCodes.Ldloc_S, obj);
            il.Emit(OpCodes.Ret);
            return (Func<object, DbParameter[]>)dm.CreateDelegate(typeof(Func<object, DbParameter[]>));
        }

    }
}
