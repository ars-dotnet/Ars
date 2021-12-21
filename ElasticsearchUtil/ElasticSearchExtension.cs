﻿using ElasticsearchUtil.Configs;
using Microsoft.Extensions.DependencyInjection;
using MyArsenal.Commom.Tool;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticsearchUtil
{
    public static class ElasticSearchExtension
    {
        public static void AddEs(this MyArsenalBaseConfig config, Action<ElasticSearchConfig> action) 
        {
            if (null == action)
                throw new ArgumentNullException(nameof(action));

            config.AddConfigExtension(new ElasticSearchExtensionConfig(action));
        }
    }
}