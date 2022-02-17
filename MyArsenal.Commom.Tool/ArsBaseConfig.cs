using System;
using System.Collections.Generic;
using System.Text;

namespace MyArsenal.Commom.Tool
{
    public class ArsBaseConfig
    {
        public ArsBaseConfig()
        {
            configExtensions = new List<IArsServiceCoreExtension>();
        }

        internal IList<IArsServiceCoreExtension> configExtensions { get; }

        public void AddConfigExtension(IArsServiceCoreExtension configExtension) 
        {
            if (null == configExtension)
                throw new ArgumentNullException(nameof(configExtension));

            configExtensions.Add(configExtension);
        }
    }
}
