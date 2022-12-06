/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

using Loxodon.Log;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;

namespace Loxodon.Framework.Observables
{
    /// <summary>
    /// 可观测的对象, 一般 Model 需要继承 ObservableObject
    /// </summary>
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ObservableObject));

        private static readonly PropertyChangedEventArgs NULL_EVENT_ARGS = new PropertyChangedEventArgs(null);

        /// <summary>
        /// 属性名 - 属性对应事件 字典
        /// </summary>
        private static readonly Dictionary<string, PropertyChangedEventArgs> PROPERTY_EVENT_ARGS = new Dictionary<string, PropertyChangedEventArgs>();

        /// <summary>
        /// 根据属性名称查找属性事件
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        private static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (propertyName == null)
                return NULL_EVENT_ARGS;

            // 字典查找
            PropertyChangedEventArgs eventArgs;
            if (PROPERTY_EVENT_ARGS.TryGetValue(propertyName, out eventArgs))
                return eventArgs;

            // 没有的话新建一个
            eventArgs = new PropertyChangedEventArgs(propertyName);
            PROPERTY_EVENT_ARGS[propertyName] = eventArgs;
            return eventArgs;
        }

        // 进程锁
        private readonly object _lock = new object();

        private PropertyChangedEventHandler propertyChanged;
        /// <summary>
        /// 属性变化时的代理方法
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { lock (_lock) { this.propertyChanged += value; } }
            remove { lock (_lock) { this.propertyChanged -= value; } }
        }

        //[Conditional("DEBUG")]
        //protected void VerifyPropertyName(string propertyName)
        //{
        //    var type = this.GetType();
        //    if (!string.IsNullOrEmpty(propertyName) && type.GetProperty(propertyName) == null)
        //        throw new ArgumentException("Property not found", propertyName);
        //}

        /// <summary>
        /// 根据属性名称，触发属性变动事件
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            RaisePropertyChanged(GetPropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs">Property changed event.</param>
        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            try
            {
                if (propertyChanged != null)
                    propertyChanged(this, eventArgs);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Set property '{0}', raise PropertyChanged failure.Exception:{1}", eventArgs.PropertyName, e);
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void RaisePropertyChanged(params PropertyChangedEventArgs[] eventArgs)
        {
            foreach (var args in eventArgs)
            {
                try
                {
                    if (propertyChanged != null)
                        propertyChanged(this, args);
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Set property '{0}', raise PropertyChanged failure.Exception:{1}", args.PropertyName, e);
                }
            }
        }

        /// <summary>
        /// 通过反射的方法, 解析 Lambda 表达式中的属性名
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        protected virtual string ParserPropertyName(LambdaExpression propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", "propertyExpression");

            var property = body.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Argument is not a property", "propertyExpression");

            return property.Name;
        }

        /// <summary>
        /// 设置指定的属性, field = newValue, propertyExpression 属性表达式
        /// </summary>
        protected bool Set<T>(ref T field, T newValue, Expression<Func<T>> propertyExpression)
        {
            if (object.Equals(field, newValue))
                return false;

            field = newValue;

            var propertyName = ParserPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// 设置指定的属性, field = newValue, CallerMemberName 避免传入属性的名称
        /// </summary>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(field, newValue))
                return false;

            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #region Set propertyName 泛型方法的特例

        protected bool Set(ref bool field, bool newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref byte field, byte newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref sbyte field, sbyte newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref char field, char newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref DateTime field, DateTime newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref short field, short newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref ushort field, ushort newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref int field, int newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref uint field, uint newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref long field, long newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref ulong field, ulong newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref float field, float newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref double field, double newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref decimal field, decimal newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref Vector2 field, Vector2 newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref Vector3 field, Vector3 newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref Vector4 field, Vector4 newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref Color field, Color newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref Rect field, Rect newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        protected bool Set(ref Quaternion field, Quaternion newValue, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, newValue, propertyName);
        }

        #endregion

        /// <summary>
        /// 设置指定的属性, field = newValue, PropertyChangedEventArgs
        /// </summary>
        protected bool Set<T>(ref T field, T newValue, PropertyChangedEventArgs eventArgs)
        {
            if (object.Equals(field, newValue))
                return false;

            field = newValue;
            RaisePropertyChanged(eventArgs);
            return true;
        }

        #region Set PropertyChangedEventArgs 泛型方法特例

        protected bool Set(ref bool field, bool newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref char field, char newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref DateTime field, DateTime newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref byte field, byte newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref sbyte field, sbyte newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref short field, short newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref ushort field, ushort newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref int field, int newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref uint field, uint newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref long field, long newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref ulong field, ulong newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref float field, float newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref double field, double newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref decimal field, decimal newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref Color field, Color newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref Vector2 field, Vector2 newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref Vector3 field, Vector3 newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref Vector4 field, Vector4 newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref Quaternion field, Quaternion newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        protected bool Set(ref Rect field, Rect newValue, PropertyChangedEventArgs eventArgs)
        {
            return this.SetValue(ref field, newValue, eventArgs);
        }

        #endregion

        /// <summary>
        /// 判断是否是同一个对象, 然后进行设置并触发事件
        /// </summary>
        protected bool SetValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null) where T : IEquatable<T>
        {
            if ((field != null && field.Equals(newValue)) || (field == null && newValue == null))
                return false;

            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 判断是否是同一个对象, 然后进行设置并触发事件
        /// </summary>
        protected bool SetValue<T>(ref T field, T newValue, PropertyChangedEventArgs eventArgs) where T : IEquatable<T>
        {
            if ((field != null && field.Equals(newValue)) || (field == null && newValue == null))
                return false;

            field = newValue;
            RaisePropertyChanged(eventArgs);
            return true;
        }
    }
}
