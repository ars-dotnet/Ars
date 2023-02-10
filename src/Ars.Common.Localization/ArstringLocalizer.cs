using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization
{
    public class ArstringLocalizer : IArstringLocalizer
    {
        private readonly IStringLocalizer _localizer;
        public ArstringLocalizer(IStringLocalizerFactory stringLocalizerFactory)
        {
            var assemblyname = new AssemblyName(Assembly.GetEntryAssembly()!.FullName!);
            _localizer = stringLocalizerFactory.Create(nameof(ArshareResource), assemblyname.Name!);
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name,arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }
    }
}
