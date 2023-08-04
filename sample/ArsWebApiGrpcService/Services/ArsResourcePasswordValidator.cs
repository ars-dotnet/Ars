using IdentityServer4.Validation;

namespace WebApiGrpcServices.Services
{
    public class ArsResourcePasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            return Task.CompletedTask;
        }
    }
}
