#region File and License Information
/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2011-01-24 20:41:43Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using System.Text;

namespace Pages.Helpers
{
    #region BindingChangedEventArgs
    public class BindingChangedEventArgs : EventArgs
    {
        public BindingChangedEventArgs(DependencyPropertyChangedEventArgs e)
        {
            EventArgs = e;
        }

        public DependencyPropertyChangedEventArgs EventArgs { get; private set; }
    }
    #endregion

    #region DelegateCommand
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(
            Action<object> executeAction, Func<object, bool> canExecute)
            : base(executeAction, canExecute)
        {
            /* Intentionally left blank. */
        }

        public DelegateCommand(Action<object> executeAction)
            : base(executeAction)
        {
            /* Intentionally left blank. */
        }
    }
    #endregion

    #region DelegateCommand<T>
    public class DelegateCommand<T> : IEventCommand
    {
        string eventName = "Click";

        public string EventName
        {
            get
            {
                return eventName;
            }
            set
            {
                eventName = value;
            }
        }

        readonly Action<T> executeAction;
        readonly Func<T, bool> canExecuteFunc;
        bool previousCanExecute;

        public DelegateCommand(Action<T> executeAction, Func<T, bool> canExecuteFunc)
        {
            this.executeAction = ArgumentValidator.AssertNotNull(executeAction, "executeAction");
            this.canExecuteFunc = canExecuteFunc;
        }

        public DelegateCommand(Action<T> executeAction)
        {
            this.executeAction = ArgumentValidator.AssertNotNull(executeAction, "executeAction");
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            object coercedParameter = CoerceParameterToType(parameter);
            return CanExecute((T)coercedParameter);
        }

        public void Execute(object parameter)
        {
            object coercedParameter = CoerceParameterToType(parameter);
            Execute((T)coercedParameter);
        }

        object CoerceParameterToType(object parameter)
        {
            object coercedParameter = parameter;
            Type typeOfT = typeof(T);
            if (parameter != null && !typeOfT.IsAssignableFrom(parameter.GetType()))
            {
                coercedParameter = ImplicitTypeConverter.ConvertToType(parameter, typeOfT);
            }
            return coercedParameter;
        }

        #endregion

        public bool CanExecute(T parameter)
        {
            if (canExecuteFunc == null)
            {
                return true;
            }

            bool temp = canExecuteFunc(parameter);

            if (previousCanExecute != temp)
            {
                previousCanExecute = temp;
                OnCanExecuteChanged(EventArgs.Empty);
            }

            return previousCanExecute;
        }

        public void Execute(T parameter)
        {
            executeAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var tempEvent = CanExecuteChanged;
            if (tempEvent != null)
            {
                tempEvent(this, e);
            }
        }
    }
    #endregion

    #region ArgumentValidator
    public static class ArgumentValidator
    {
        /// <summary>
        /// Ensures the specified value is not null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The specified value.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the specified value 
        /// is <code>null</code>.</exception>
        public static T AssertNotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Ensures the specified value is not <code>null</code> 
        /// or empty (a zero length string).
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The specified value.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the specified value 
        /// is <code>null</code> or empty (a zero length string).</exception>
        public static string AssertNotNullOrEmpty(string value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.Length < 1)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentException(
                    "Parameter should not be an empty string.", parameterName);
            }

            return value;
        }

        /// <summary>
        /// Ensures the specified value is not <code>null</code> 
        /// and that it is of the specified type.
        /// </summary>
        /// <param name="value">The value to test.</param> 
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value to test.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the specified value 
        /// is <code>null</code> or of type not assignable 
        /// from the specified type.</exception>
        /// <example>
        /// public DoSomething(object message)
        /// {
        /// 	this.message = ArgumentValidator.AssertNotNullAndOfType&lt;string&gt;(
        ///							message, "message");	
        /// }
        /// </example>
        public static T AssertNotNullAndOfType<T>(
            object value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            var result = value as T;
            if (result == null)
            {
                throw new ArgumentException(string.Format(
                    "Expected argument of type {0}, but was {1}",
                    typeof(T), value.GetType()),
                    parameterName);
            }
            return result;
        }

        /* TODO: [DV] Comment. */
        public static int AssertGreaterThan(
            int comparisonValue, int value, string parameterName)
        {
            if (value <= comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be greater than "
                    + comparisonValue, parameterName);
            }
            return value;
        }

        /* TODO: [DV] Comment. */
        public static double AssertGreaterThan(
            double comparisonValue, double value, string parameterName)
        {
            if (value <= comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be greater than "
                    + comparisonValue, parameterName);
            }
            return value;
        }

        /* TODO: [DV] Comment. */
        public static long AssertGreaterThan(
            long comparisonValue, long value, string parameterName)
        {
            if (value <= comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be greater than "
                    + comparisonValue, parameterName);
            }
            return value;
        }

        /* TODO: [DV] Comment. */
        public static int AssertGreaterThanOrEqualTo(
            int comparisonValue, int value, string parameterName)
        {
            if (value < comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be greater than or equal to "
                    + comparisonValue, parameterName);
            }
            return value;
        }

        /* TODO: [DV] Comment. */
        public static double AssertGreaterThanOrEqualTo(
            double comparisonValue, double value, string parameterName)
        {
            if (value < comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be greater than or equal to "
                    + comparisonValue, parameterName);
            }
            return value;
        }

        /* TODO: [DV] Comment. */
        public static long AssertGreaterThanOrEqualTo(
            long comparisonValue, long value, string parameterName)
        {
            if (value < comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be greater than "
                    + comparisonValue, parameterName);
            }
            return value;
        }

        /* TODO: [DV] Comment. */
        public static double AssertLessThan(
            double comparisonValue, double value, string parameterName)
        {
            if (value >= comparisonValue)
            {
                /* TODO: Make localizable resource. */
                throw new ArgumentOutOfRangeException(
                    "Parameter should be less than "
                    + comparisonValue, parameterName);
            }
            return value;
        }

    }
    #endregion

    #region ImplicitTypeConverter
    static class ImplicitTypeConverter
    {
        public static object ConvertToType(object value, Type type)
        {
            ArgumentValidator.AssertNotNull(type, "type");

            if (value == null || type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            TypeConverter converter = GetTypeConverter(type);

            if (converter != null && converter.CanConvertFrom(value.GetType()))
            {
                value = converter.ConvertFrom(value);
                return value;
            }

            return null;
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            var attribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(
                                type, typeof(TypeConverterAttribute), false);

            if (attribute != null)
            {
                try
                {
                    Type converterType = Type.GetType(attribute.ConverterTypeName, false);
                    if (converterType != null)
                    {
                        return Activator.CreateInstance(converterType) as TypeConverter;
                    }
                }
                catch (Exception)
                {
                    /* Suppress. */
                }
            }
            return new FromStringConverter(type);
        }
    }

    #region FromStringConverter
    public class FromStringConverter : TypeConverter
    {
        readonly Type type;

        public FromStringConverter(Type type)
        {
            this.type = type;
        }

        public override bool CanConvertFrom(
            ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                    || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (type == typeof(bool))
                {
                    return bool.Parse(stringValue);
                }

                if (type.IsEnum)
                {
                    return Enum.Parse(type, stringValue, false);
                }

                if (type == typeof(Guid))
                {
                    return new Guid(stringValue);
                }

                var stringBuilder = new StringBuilder();
                stringBuilder.Append("<ContentControl xmlns='http://schemas.microsoft.com/client/2007' xmlns:c='"
                                     + ("clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Split(new[] { ',' })[0]) + "'>\n");
                stringBuilder.Append("<c:" + type.Name + ">\n");
                stringBuilder.Append(stringValue);
                stringBuilder.Append("</c:" + type.Name + ">\n");
                stringBuilder.Append("</ContentControl>");
                var instance = XamlReader.Load(stringBuilder.ToString()) as ContentControl;
                if (instance != null)
                {
                    return instance.Content;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    #endregion

    #endregion

    #region IEventCommand
    public interface IEventCommand : ICommand
    {
        string EventName { get; }
    }
    #endregion

    #region DependencyPropertyListener
    public class DependencyPropertyListener
    {
        static int index;
        readonly DependencyProperty property;
        FrameworkElement target;

        public DependencyPropertyListener()
        {
            property = DependencyProperty.RegisterAttached(
                "DependencyPropertyListener" + index++,
                typeof(object),
                typeof(DependencyPropertyListener),
                new PropertyMetadata(null, HandleValueChanged));
        }

        public event EventHandler<BindingChangedEventArgs> Changed;

        void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OnChanged(new BindingChangedEventArgs(e));
        }

        protected void OnChanged(BindingChangedEventArgs e)
        {
            var temp = Changed;
            if (temp != null)
            {
                temp(target, e);
            }
        }

        public void Attach(FrameworkElement element, Binding binding)
        {
            if (target != null)
            {
                throw new Exception(
                    "Cannot attach an already attached listener");
            }

            target = element;
            target.SetBinding(property, binding);
        }

        public void Detach()
        {
            target.ClearValue(property);
            target = null;
        }
    }
    #endregion

    #region ScrollViewerMonitor

    public class ScrollViewerMonitor
    {
        public static DependencyProperty AtEndCommandProperty
            = DependencyProperty.RegisterAttached(
                "AtEndCommand", typeof(ICommand),
                typeof(ScrollViewerMonitor),
                new PropertyMetadata(OnAtEndCommandChanged));

        public static ICommand GetAtEndCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(AtEndCommandProperty);
        }

        public static void SetAtEndCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(AtEndCommandProperty, value);
        }


        public static void OnAtEndCommandChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            if (element != null)
            {
                element.Loaded -= element_Loaded;
                element.Loaded += element_Loaded;
            }
        }

        static void element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= element_Loaded;
            ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(element);
            if (scrollViewer == null)
            {
                throw new InvalidOperationException("ScrollViewer not found.");
            }

            var listener = new DependencyPropertyListener();
            listener.Changed
                += delegate
                {
                    bool atBottom = scrollViewer.VerticalOffset
                                        >= scrollViewer.ScrollableHeight;

                    if (atBottom)
                    {
                        var atEnd = GetAtEndCommand(element);
                        if (atEnd != null)
                        {
                            atEnd.Execute(null);
                        }
                    }
                };
            Binding binding = new Binding("VerticalOffset") { Source = scrollViewer };
            listener.Attach(scrollViewer, binding);
        }

        static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }
    }

    //public class ScrollViewerMonitor
    //{
    //    public static DependencyProperty AtEndCommandProperty
    //        = DependencyProperty.RegisterAttached(
    //            "AtEndCommand", typeof(ICommand),
    //            typeof(ScrollViewerMonitor),
    //            new PropertyMetadata(OnAtEndCommandChanged));

    //    public static ICommand GetAtEndCommand(DependencyObject obj)
    //    {
    //        return (ICommand)obj.GetValue(AtEndCommandProperty);
    //    }

    //    public static void SetAtEndCommand(DependencyObject obj, ICommand value)
    //    {
    //        obj.SetValue(AtEndCommandProperty, value);
    //    }


    //    public static void OnAtEndCommandChanged(
    //        DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        FrameworkElement element = (FrameworkElement)d;
    //        if (element != null)
    //        {
    //            element.Loaded -= element_Loaded;
    //            element.Loaded += element_Loaded;
    //        }
    //    }

    //    static void element_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        FrameworkElement element = (FrameworkElement)sender;
    //        element.Loaded -= element_Loaded;
    //        ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(element);
    //        if (scrollViewer == null)
    //        {
    //            throw new InvalidOperationException("ScrollViewer not found.");
    //        }

    //        var listener = new DependencyPropertyListener();
    //        listener.Changed
    //            += delegate
    //            {
    //                bool atBottom = scrollViewer.VerticalOffset
    //                                    >= scrollViewer.ScrollableHeight;

    //                if (atBottom)
    //                {
    //                    var atEnd = GetAtEndCommand(element);
    //                    if (atEnd != null)
    //                    {
    //                        atEnd.Execute(null);
    //                    }
    //                }
    //                else if (element.Name == "MyFloatsListBox")
    //                {
    //                    var diff = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
    //                    if (diff < 0.25)
    //                    {
    //                        var atEnd = GetAtEndCommand(element);
    //                        if (atEnd != null)
    //                        {
    //                            atEnd.Execute(null);
    //                        }
    //                    }
    //                }
    //            };
    //        Binding binding = new Binding("VerticalOffset") { Source = scrollViewer };
    //        listener.Attach(scrollViewer, binding);
    //    }

    //    static T FindChildOfType<T>(DependencyObject root) where T : class
    //    {
    //        var queue = new Queue<DependencyObject>();
    //        queue.Enqueue(root);

    //        while (queue.Count > 0)
    //        {
    //            DependencyObject current = queue.Dequeue();
    //            for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
    //            {
    //                var child = VisualTreeHelper.GetChild(current, i);
    //                var typedChild = child as T;
    //                if (typedChild != null)
    //                {
    //                    return typedChild;
    //                }
    //                queue.Enqueue(child);
    //            }
    //        }
    //        return null;
    //    }
    //}
    #endregion
}
