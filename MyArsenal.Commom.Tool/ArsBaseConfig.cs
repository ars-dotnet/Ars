using System;
using System.Collections.Generic;
using System.Text;

namespace MyArsenal.Commom.Tool
{
    public class ArsBaseConfig
    {
        public ArsBaseConfig()
        {
            configExtensions = new List<IConfigExtension>();
        }

        internal IList<IConfigExtension> configExtensions { get; }

        public void AddConfigExtension(IConfigExtension configExtension) 
        {
            if (null == configExtension)
                throw new ArgumentNullException(nameof(configExtension));

            configExtensions.Add(configExtension);
        }
    }
}
