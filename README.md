# MyArs
[![NuGet](https://img.shields.io/nuget/v/Ars.Common.Host.svg)](https://www.nuget.org/packages/Ars.Common.Host/)

a simple .net6 webapiframework with some extensions.\
include autofac,consul,grpc,efcore,identityserver4,redis,signalr,localization,upload and download excel extension. \
and some samples with them.

## Getting Started

### NuGet

myars can be installed in your project with the following command.

```
PM> Install-Package Ars.Common.Host
```

myars supports some extensions, following packages are available to install:

```
PM> Install-Package Ars.Common.IdentityServer4
PM> Install-Package Ars.Common.Consul
PM> Install-Package Ars.Common.EFCore
PM> Install-Package Ars.Common.Redis
PM> Install-Package Ars.Common.SignalR
```

### Configuration
#### add Service:

    var builder = WebApplication.CreateBuilder(args);
    builder.Services
     //add ars service
     .AddArserviceCore(builder.Host, config =>
     {
         //add consul client service
         config.AddArsConsulDiscoverClient();
	 
         //add consul register service
         config.AddArsConsulRegisterServer();

         //add resource identity service
         config.AddArsIdentityClient();
	 
         //add identity server service
         config.AddArsIdentityServer();

         //add redis service
         config.AddArsRedis();

         //add localization service
         config.AddArsLocalization();
    })
    //add dbcontext service
    .AddArsDbContext<xxxDbContext>();

#### use Application:

    var app = builder.Build();
   
    //use ars core application
    app.UseArsCore();

#### change your appsettings.Development.json
 
	{
	  "Logging": {
		"LogLevel": {
		  "Default": "Information",
		  "Microsoft.AspNetCore": "Warning"
		}
	  },
	  //consul client config
	  "ConsulDiscoverConfiguration": {
		"ConsulDiscovers": [
		  {
			"ConsulAddress": "http://ip:port",
			"ServiceName": "apigrpc",
			"UseHttp1Protocol": true,
			"UseHttps": true,
			"CertificatePath": "xxx",
			"CertificatePassWord": "xxx",
			"UseIdentityServer4Valid": true,
			"IdentityServer4Address": "http://ip:port",
			"ClientId": "grpc-key",
			"ClientSecret": "grpc-secret",
			"Scope": "grpcapi-scope",
			"GrantType": "client_credentials"
		  }
		]
	  },
	  
	  //consul register config
	  "ConsulRegisterConfiguration": {
		"ConsulAddress": "http://ip:port",
		"ServiceName": "apigrpc",
		"ServiceIp": "ip",
		"ServicePort": port,
		"UseHttps": true,
		"CertificatePath": "xxx",
		"CertificatePassWord": "xxx"
	  },
	  
	  //resource server identity config
	  "ArsIdentityClientConfiguration": {
		"Authority": "http://ip:port",
		"ApiName": "apiIds4Second", 
		"RequireHttpsMetadata": true,
		"CertificatePath": "xxx",
		"CertificatePassWord": "xxx"
	  },
	  
	  //identity server config
	  "ArsIdentityServerConfiguration": {
		"ArsApiResources": [
		  {
			"Name": "apiIds4First",
			"DisplayName": "my default grpcapi",
			"UserClaims": [
			  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
			  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
			  "tenant",
			  "sub"
			],
			"Scopes": [
			  "grpcapi-scope"
			]
		  },
		  {
			"Name": "apiIds4Second",
			"DisplayName": "my default pckeapi",
			"UserClaims": [
			  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
			  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
			  "tenant",
			  "sub"
			],
			"Scopes": [
			  "ids4-scope",
			  "openid",
			  "profile"
			]
		  }
		],
		"ArsClients": [
		  {
			"AppKey": "grpc-key",
			"AppSecret": "grpc-secret",
			"AccessTokenLifetime": 3600,
			"AllowedScopes": [ "grpcapi-scope" ],
			"GrantType": [ "client_credentials", "password" ],
			"AllowedCorsOrigins": [ "http://ip:port" ]
		  },
		  {
			"AppKey": "pcke-key",
			"AppSecret": "pcke-secret",
			"AccessTokenLifetime": 3600,
			"AllowedScopes": [ "openid", "profile", "ids4-scope" ],
			"GrantType": [ "authorization_code" ],
			"RedirectUris": [ "http://ip:port/signin-oidc" ],
			"PostLogoutRedirectUris": [ "http://ip:port/signout-callback-oidc" ]
		  }
		],
		"ArsApiScopes": [
		  "grpcapi-scope",
		  "ids4-scope",
		  "openid",
		  "profile"
		],
		"CertPath": "Certificates\\IS4.pfx",
		"Password": "aabb1212"
	  },
	  
	  //redis config
	  "ArsCacheConfiguration": {
		"RedisConnection": "ip",
		"DefaultDB": 1
	  },

	  //localization config
	  "ArsLocalizationConfiguration": {
		"ResourcesPath": "Resources",
		"IsAddViewLocalization": true,
		"IsAddDataAnnotationsLocalization": true,
		"Cultures": [
		  "en-GB",
		  "en",
		  "fr-FR",
		  "fr",
		  "en-US",
		  "zh-Hans"
		],
		"DefaultRequestCulture": "en-US"
	  },
	  
	  //DbContext config
	  "ArsDbContextConfiguration": {
		//1 mysql;2 mssql
		"DbType": 2,
		//Database address
		"DefaultString": "Data Source=xxx; Initial Catalog=xxx;user id=xxx;pwd=xxx"
	  }
	}

