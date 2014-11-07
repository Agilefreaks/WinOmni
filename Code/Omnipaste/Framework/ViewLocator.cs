using System;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;

namespace Omnipaste.Framework
{
    using System.Linq;
    using System.Windows.Controls;
    using OmniUI.Attributes;

    public static class ViewLocator
    {
        public static Func<Type, Type> GetViewTypeFromViewModelType;
        public static Func<string, string> GetViewTypeNameFromViewModelTypeName;

        static ViewLocator()
        {
            GetViewTypeNameFromViewModelTypeName = viewModeltypeName => viewModeltypeName.Replace("ViewModel", "View");
            GetViewTypeFromViewModelType = type =>
            {
                var viewModelTypeName = type.FullName;
                var viewTypeName = GetViewTypeNameFromViewModelTypeName(viewModelTypeName);
                var viewType = type.Assembly.GetType(viewTypeName);
                return viewType;
            };
        }

        public static object GetViewForViewModel(object viewModel)
        {
            var viewType = GetViewTypeFromViewModelType(viewModel.GetType());
            if (viewType == null)
            {
                throw new InvalidOperationException("No View found for ViewModel of type " + viewModel.GetType());
            }

            var view = ((Screen)viewModel).GetView() ?? Activator.CreateInstance(viewType);

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.DataContext = viewModel;
            }

            InitializeComponent(view);

            return view;
        }

        private static void InitializeComponent(object element)
        {
            var method = element.GetType().GetMethod("InitializeComponent", BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                return;
            }
            method.Invoke(element, null);
        }

        public static UIElement LocateForModelType(Type modelType, DependencyObject displayLocation, object context)
        {
            var useViewAttributes =
                modelType.GetCustomAttributes(typeof(UseViewAttribute), true).Cast<UseViewAttribute>().ToArray();

            string viewTypeName;

            if (useViewAttributes.Count() == 1)
            {
                var attribute = useViewAttributes.First();
                viewTypeName = attribute.IsFullyQualifiedName
                                   ? attribute.ViewName
                                   : GetFullyQualifiedViewName(modelType, attribute);
            }
            else
            {
                viewTypeName = modelType.FullName.Replace("Model", string.Empty);
            }

            if (context != null)
            {
                viewTypeName = viewTypeName.Remove(viewTypeName.Length - 4, 4);
                viewTypeName = viewTypeName + "." + context;
            }

            var viewType = (from assembly in AssemblySource.Instance
                            from type in assembly.GetExportedTypes()
                            where type.FullName == viewTypeName
                            select type).FirstOrDefault();

            return viewType == null
                       ? new TextBlock { Text = string.Format("{0} not found.", viewTypeName) }
                       : Caliburn.Micro.ViewLocator.GetOrCreateViewType(viewType);

        }

        private static string GetFullyQualifiedViewName(Type modelType, UseViewAttribute attribute)
        {
            return string.Concat(
                (modelType.Namespace ?? string.Empty).Replace("Model", string.Empty),
                ".",
                attribute.ViewName);
        }
    }
}