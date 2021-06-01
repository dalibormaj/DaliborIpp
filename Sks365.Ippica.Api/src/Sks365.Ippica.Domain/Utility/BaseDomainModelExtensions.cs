using AutoMapper;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Domain.Model;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Sks365.Ippica.Domain.Utility
{
    public static class BaseDomainModelExtensions
    {
        /// <summary>
        /// Apply new (NOT NULL) values to the model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="newModel">Model to apply</param>
        /// <returns></returns>
        public static T ApplyNewValues<T>(this T model, T newModel) where T : BaseDomainModel
        {
            var applyNewValuesMethod =
                (
                    from m in typeof(BaseDomainModelExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    where m.Name == nameof(ApplyNewValues)
                    let p = m.GetParameters()
                    where p.Length == 2
                        && p[0].ParameterType.Name.Equals(nameof(T))
                        && p[1].ParameterType.Name.Equals(nameof(T))
                    select m
                ).FirstOrDefault();


            var propertyInfos = typeof(T).GetProperties();
            if (model == null) return newModel;

            if (newModel != null)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    var currentValue = typeof(T).GetProperty(propertyInfo.Name).GetValue(model, null);
                    var newValue = typeof(T).GetProperty(propertyInfo.Name).GetValue(newModel, null);

                    if (newValue != null)
                    {
                        var newValueType = newValue.GetType();

                        if ((newValueType.IsValueType || newValueType == typeof(string)))
                        {
                            propertyInfo.SetValue(model, newValue);
                        }
                        else if (newValue is BaseDomainModel)
                        {
                            var itemType = newValue.GetType();
                            var applyNewValuesGenericMethod = applyNewValuesMethod.MakeGenericMethod(itemType);
                            applyNewValuesGenericMethod.Invoke(null, new object[] { currentValue, newValue });

                        }
                        else if (newValue is ICollection && (newValue as ICollection).Count > 0)
                        {
                            var currentValueArray = new ArrayList((ICollection)currentValue);
                            var newValueArray = new ArrayList((ICollection)newValue);

                            if (currentValueArray.Count != newValueArray.Count)
                                throw new Exception("ApplyNewValues cannot be used for collections with different size. Please manually set the values");

                            //go through all elements to apply 
                            for (var i = 0; i < newValueArray.Count; i++)
                            {
                                var itemType = newValueArray[i].GetType();
                                var applyNewValuesGenericMethod = applyNewValuesMethod.MakeGenericMethod(itemType);

                                //apply new values object per object
                                applyNewValuesGenericMethod.Invoke(null, new object[] { currentValueArray[i], newValueArray[i] });
                            }
                        }
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Apply new values by the rule
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <param name="rule">Rule for applying new values</param>
        /// <returns></returns>
        public static T ApplyNewValues<T>(this T model, Func<T, T> rule) where T : BaseDomainModel
        {
            var result = rule(model);
            return result;
        }

        /// <summary>
        /// Throw an exception if a condition is not satisfied
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="condition">Condition to satisfy</param>
        /// <param name="exception">Exception to throw</param>
        /// <returns></returns>
        public static T ThrowIf<T>(this T model, Func<T, bool> condition, BaseException exception) where T : BaseDomainModel
        {
            if (condition(model))
                throw exception;

            return model;
        }

        /// <summary>
        /// Throw an exception if a condition is not satisfied
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="condition">Condition to satisfy</param>
        /// <param name="exception">Exception to throw</param>
        /// <returns></returns>
        public static T ThrowIf<T>(this T model, Func<bool> condition, BaseException exception) where T : BaseDomainModel
        {
            if (condition())
                throw exception;

            return model;
        }


        /// <summary>
        /// Compare all primitive values with the same model.
        /// Null values will be discarded from comparation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The (Not null) values of the modelToCompare will be compared with the same value from the model</param>
        /// <param name="modelToCompare">Model to compare, usually model from DB</param>
        /// <returns>If objects are the same it will be returned true, otherwise false</returns>
        public static bool Compare<T>(this T model, T modelToCompare) where T : BaseDomainModel
        {

            var isTheSame = true;
            var propertyInfos = typeof(T).GetProperties();

            if (model != null && modelToCompare != null)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {

                    var value = typeof(T).GetProperty(propertyInfo.Name).GetValue(model, null);
                    var valueToCompare = typeof(T).GetProperty(propertyInfo.Name).GetValue(modelToCompare, null);

                    if (valueToCompare != null)
                    {
                        //comparing attributes have to exist
                        isTheSame = value != null && valueToCompare != null;
                        if (!isTheSame) break;

                        var type = value.GetType();
                        var typeToCompare = valueToCompare.GetType();

                        //comparing attributes have to be of the same type
                        isTheSame = type.Equals(typeToCompare);
                        if (!isTheSame) break;

                        //Comparing action
                        if ((type.IsValueType || type == typeof(string)))
                        {
                            if (type == typeof(DateTime))
                                isTheSame = value.ToString().Equals(valueToCompare.ToString());
                            else
                                isTheSame = value.Equals(valueToCompare);

                            if (!isTheSame) break;
                        }
                        else if (value is BaseDomainModel)
                        {
                            //recursion, call Compare again
                            MethodInfo compareMethod = typeof(BaseDomainModelExtensions).GetMethod(nameof(Compare))
                                                                                        .MakeGenericMethod(type);
                            var isTheSameObject = compareMethod.Invoke(null, new object[] { value, valueToCompare });
                            isTheSame = (bool)isTheSameObject;
                            if (!isTheSame) break;
                        }
                        else if (value is ICollection)
                        {
                            //go through all elements and compare
                            isTheSame = ((ICollection)value).CompareList((ICollection)valueToCompare);
                            if (!isTheSame) break;
                        }
                    }
                }
            }
            else if (model == null && modelToCompare == null)
            {
                isTheSame = true;
            }
            else
                isTheSame = false;

            return isTheSame;
        }

        public static bool CompareList(this ICollection list, ICollection listToCompare)
        {
            //count of the list has to be the same
            var isTheSame = list.Count == listToCompare.Count;
            if (!isTheSame)
                return isTheSame;

            //go through all elements and compare
            foreach (var item in list)
            {
                if (item != null)
                {
                    var type = item.GetType();

                    if (type.IsSubclassOf(typeof(BaseDomainModel)))
                    {
                        MethodInfo compareMethod = typeof(BaseDomainModelExtensions).GetMethod(nameof(Compare))
                                                                                    .MakeGenericMethod(type);

                        var isTheSameItem = false; //default is false, if compare is successful break, otherwise stay false. At the end we'll know the result
                        foreach (var itemToCompare in listToCompare)
                        {
                            //recursion, call Compare again
                            var isTheSameObject = compareMethod.Invoke(null, new object[] { item, itemToCompare });
                            isTheSameItem = (bool)isTheSameObject;

                            if (isTheSameItem) break;
                        }

                        isTheSame = isTheSameItem;
                        if (!isTheSame) break;
                    }
                    else throw new TypeAccessException("Compare method allowed just for types derived from BaseDomainModel");
                }
            }

            return isTheSame;
        }

        public static T Clone<T>(this T model, Action<T> adjust) where T : BaseDomainModel
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.CreateMap<T, T>();
            }));
            var cloned = mapper.Map<T>(model);

            if (adjust != null)
            {
                adjust.Invoke(cloned);
            }

            return cloned;
        }

        public static T Clone<T>(this T model) where T : BaseDomainModel
        {
            return model.Clone(adjust: null);
        }
    }
}
