using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ars.Commom.Tool.Extension;
using Ars.Common.IdentityServer4.options;
using IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Ars.Common.IdentityServer4.Extension
{
    internal static class ArsIdentityServerBuilderExtension
    {
        internal static IIdentityServerBuilder AddArsIdentityResource(this IIdentityServerBuilder builder) 
        {
            var implementationInstance = new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
            builder.Services.AddSingleton(implementationInstance);
            builder.AddResourceStore<InMemoryResourcesStore>();
            return builder;
        }

        internal static IIdentityServerBuilder AddArsApiResource(this IIdentityServerBuilder builder, IEnumerable<ArsIdentityServerOption.ArsApiResource> apiResources) 
        {
            if(!apiResources.HasValue())
                return builder;

            IEnumerable<ApiResource> resources = apiResources.Select(
                r => 
                new ApiResource(r.Name, r.DisplayName, r.UserClaims) { Scopes = r.Scopes ?? Array.Empty<string>() });
            builder.Services.AddSingleton(resources);
            builder.AddResourceStore<InMemoryResourcesStore>();

            return builder;
        }

        internal static IIdentityServerBuilder AddArsClients(this IIdentityServerBuilder builder,IEnumerable<ArsIdentityServerOption.ArsApiClient> arsApiClients) 
        {
            if (!arsApiClients.HasValue())
                return builder;

            IEnumerable<Client> client = CastArsClientToClient(arsApiClients);
            builder.Services.AddSingleton(client);
            builder.AddClientStore<InMemoryClientStore>();
            var serviceDescriptor =
                builder.Services.LastOrDefault(
                    x => x.ServiceType == typeof(ICorsPolicyService));
            if (serviceDescriptor != null &&
                serviceDescriptor.ImplementationType == typeof(DefaultCorsPolicyService) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient)
                builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();

            return builder;
        }

        private static IEnumerable<Client> CastArsClientToClient(
            IEnumerable<ArsIdentityServerOption.ArsApiClient> clients)
        {
            var clientList = new List<Client>();
            foreach (var client in clients)
            {
                var client1 = new Client { ClientId = client.AppKey };
                client1.ClientSecrets.Add(new Secret(client.AppSecret.Sha256(), new DateTime?()));
                client1.AllowedScopes = client.AllowedScopes;
                client1.AllowedGrantTypes = client.GrantType;
                client1.AllowOfflineAccess = true;
                client1.AccessTokenLifetime = (int)TimeSpan.FromDays(7.0).TotalSeconds;
                client1.RefreshTokenExpiration = TokenExpiration.Sliding;
                client1.AbsoluteRefreshTokenLifetime = 0;
                client1.SlidingRefreshTokenLifetime = (int)TimeSpan.FromDays(365.0).TotalSeconds;
                client1.AllowAccessTokensViaBrowser = true;//控制是否通过浏览器传输此客户端的访问令牌
                                                           //登录成功跳转地址
                client1.RedirectUris = client.RedirectUris;
                //退出登录跳转地址
                client1.PostLogoutRedirectUris = client.PostLogoutRedirectUris;
                //跨域地址
                client1.AllowedCorsOrigins = client.AllowedCorsOrigins;

                clientList.Add(client1);
            }

            return clientList;
        }

        internal static IIdentityServerBuilder AddArsScopes(
            this IIdentityServerBuilder builder,
            IEnumerable<string> scopes)
        {
            if (!scopes.HasValue())
                return builder;

            IEnumerable<ApiScope> apiScopes = scopes.Select(r => new ApiScope(r));

            builder.Services.AddSingleton(apiScopes);
            builder.AddResourceStore<InMemoryResourcesStore>();

            return builder;
        }

        internal static IIdentityServerBuilder AddArsSigningCredential(this IIdentityServerBuilder builder, X509Certificate2 certificate, string signingAlgorithm = "RS256") 
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            if (!certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("X509 certificate does not have a private key.");
            }

            X509SecurityKey x509SecurityKey = new X509SecurityKey(certificate);
            x509SecurityKey.KeyId += signingAlgorithm;
            SigningCredentials credential = new SigningCredentials(x509SecurityKey, signingAlgorithm);
            return builder.AddSigningCredential(credential);
        }
    }
}
