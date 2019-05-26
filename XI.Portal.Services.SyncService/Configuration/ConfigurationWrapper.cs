﻿using System.Configuration;

namespace XI.Portal.Services.SyncService.Configuration
{
    public class ConfigurationWrapper : IConfigurationWrapper
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}