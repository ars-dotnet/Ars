using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Ars.Common.Core.Localization.ValidProvider
{
    public class ValidationMetadataLocalizationProvider : IValidationMetadataProvider
    {
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var validators = context.ValidationMetadata.ValidatorMetadata;

            // add [Required] for value-types (int/DateTime etc)
            // to set ErrorMessage before asp.net does it
            var theType = context.Key.ModelType;
            var underlyingType = Nullable.GetUnderlyingType(theType);

            if (theType.IsValueType &&
                underlyingType == null && // not nullable type
                validators.All(m => m.GetType() != typeof(RequiredAttribute)))
            {
                validators.Add(new RequiredAttribute());
            }
            foreach (var obj in validators)
            {
                if (!(obj is ValidationAttribute attribute))
                {
                    continue;
                }
                if (attribute.ErrorMessage == null && attribute.ErrorMessageResourceName == null)
                {
                    attribute.ErrorMessage = $"{attribute.GetType().Name.Replace("Attribute", "")}_ValidationError";
                }
            }
        }
    }
}
