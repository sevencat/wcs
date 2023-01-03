﻿namespace fineyun.wcs.support.db;

using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

public static class FreeSqlJsonMapCoreExtensions
{
    static int _isAoped = 0;
    static readonly ConcurrentDictionary<Type, bool> _dicTypes = new();
    static readonly MethodInfo MethodJsonConvertDeserializeObject = typeof(JsonConvert).GetMethod("DeserializeObject", new[] { typeof(string), typeof(Type) });
    static readonly MethodInfo MethodJsonConvertSerializeObject = typeof(JsonConvert).GetMethod("SerializeObject", new[] { typeof(object), typeof(JsonSerializerSettings) });
    static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, bool>> _dicJsonMapFluentApi = new();
    static readonly object _concurrentObj = new();

    public static ColumnFluent JsonMap(this ColumnFluent col)
    {
        _dicJsonMapFluentApi.GetOrAdd(col._entityType, _ => new ConcurrentDictionary<string, bool>())
                            .GetOrAdd(col._property.Name, _ => true);
        return col;
    }

    /// <summary>
    /// When the entity class property is <see cref="object"/> and the attribute is marked as <see cref="JsonMapAttribute"/>, map storage in JSON format. <br />
    /// 当实体类属性为【对象】时，并且标记特性 [JsonMap] 时，该属性将以JSON形式映射存储
    /// </summary>
    /// <returns></returns>
    public static void UseJsonMap(this IFreeSql that)
    {
        UseJsonMap(that, JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings());
    }

    public static void UseJsonMap(this IFreeSql that, JsonSerializerSettings settings)
    {
        if (Interlocked.CompareExchange(ref _isAoped, 1, 0) == 0)
        {
            FreeSql.Internal.Utils.GetDataReaderValueBlockExpressionSwitchTypeFullName.Add((returnTarget, valueExp, type) =>
            {
                if (_dicTypes.ContainsKey(type)) return Expression.IfThenElse(
                    Expression.TypeIs(valueExp, type),
                    Expression.Return(returnTarget, valueExp),
                    Expression.Return(returnTarget, Expression.TypeAs(Expression.Call(MethodJsonConvertDeserializeObject, Expression.Convert(valueExp, typeof(string)), Expression.Constant(type)), type))
                );
                return null;
            });
        }

        that.Aop.ConfigEntityProperty += (_, e) =>
        {
            var isJsonMap = e.Property.GetCustomAttributes(typeof(JsonMapAttribute), false).Any() || _dicJsonMapFluentApi.TryGetValue(e.EntityType, out var tryjmfu) && tryjmfu.ContainsKey(e.Property.Name);
            if (isJsonMap)
            {
                if (FreeSql.Internal.Utils.dicExecuteArrayRowReadClassOrTuple.ContainsKey(e.Property.PropertyType))
                    return; //基础类型使用 JsonMap 无效

                e.ModifyResult.MapType = typeof(string);
                e.ModifyResult.StringLength = -2;
                if (_dicTypes.TryAdd(e.Property.PropertyType, true))
                {
                    lock (_concurrentObj)
                    {
                        FreeSql.Internal.Utils.dicExecuteArrayRowReadClassOrTuple[e.Property.PropertyType] = true;
                        FreeSql.Internal.Utils.GetDataReaderValueBlockExpressionObjectToStringIfThenElse.Add((returnTarget, valueExp, elseExp, _) =>
                        {
                            return Expression.IfThenElse(
                                Expression.TypeIs(valueExp, e.Property.PropertyType),
                                Expression.Return(returnTarget, Expression.Call(MethodJsonConvertSerializeObject, Expression.Convert(valueExp, typeof(object)), Expression.Constant(settings)), typeof(object)),
                                elseExp);
                        });
                    }
                }
            }
        };
    }
}