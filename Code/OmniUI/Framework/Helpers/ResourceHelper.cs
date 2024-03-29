﻿namespace OmniUI.Framework.Helpers
{
    using System.Windows;

    public class ResourceHelper : IResourceHelper
    {
        private static IResourceHelper _instance;

        public static IResourceHelper Instance
        {
            get
            {
                return _instance ?? (_instance = new ResourceHelper());
            }
            set
            {
                _instance = value;
            }
        }

        object IResourceHelper.GetByKey(string key)
        {
            return Application.Current.FindResource(key);
        }

        public static object GetByKey(string key)
        {
            return Instance.GetByKey(key);
        }

        public static TResource GetByKey<TResource>(string key)
        {
            return (TResource)GetByKey(key);
        }
    }
}