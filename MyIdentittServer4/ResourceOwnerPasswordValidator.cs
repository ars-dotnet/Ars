using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace MyIdentittServer4
{
    /// <summary>
    /// 密码模式校验实现类
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context.UserName == "admin" && context.Password == "123456")
            {
                context.Result = new GrantValidationResult(
                 subject: context.UserName,
                 authenticationMethod: OidcConstants.AuthenticationMethods.Password);
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "invalid custom credential");
            }
            return Task.CompletedTask;
        }
    }
}
