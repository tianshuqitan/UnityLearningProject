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

namespace Loxodon.Framework.Messaging
{
    /// <summary>
    /// 消息接口,包括 Subscribe(订阅消息) Publish(发布)
    /// </summary>
    public interface IMessenger
    {
        ISubscription<object> Subscribe(Type type, Action<object> action);

        ISubscription<T> Subscribe<T>(Action<T> action);

        ISubscription<object> Subscribe(string channel, Type type, Action<object> action);

        ISubscription<T> Subscribe<T>(string channel, Action<T> action);

        void Publish(object message);

        void Publish<T>(T message);

        void Publish(string channel, object message);

        void Publish<T>(string channel, T message);

    }
}
