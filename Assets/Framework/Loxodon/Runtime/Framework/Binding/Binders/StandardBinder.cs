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

using Loxodon.Framework.Binding.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Loxodon.Framework.Binding.Binders
{
    /// <summary>
    /// 标准的 Binder, 一般在启动 BindingServiceBundle 的时候进行了注册
    /// <para> 1. 通过 BindingFactory 进行 Binding 的创建 </para>
    /// </summary>
    public class StandardBinder : IBinder
    {
        protected IBindingFactory factory;

        public StandardBinder(IBindingFactory factory)
        {
            this.factory = factory;
        }

        public IBinding Bind(IBindingContext bindingContext, object source, object target, BindingDescription bindingDescription)
        {
            return factory.Create(bindingContext, source, target, bindingDescription);
        }

        public IEnumerable<IBinding> Bind(IBindingContext bindingContext, object source, object target, IEnumerable<BindingDescription> bindingDescriptions)
        {
            if (bindingDescriptions == null)
                return new IBinding[0];

            return bindingDescriptions.Select(description => this.Bind(bindingContext, source, target, description));
        }
    }
}