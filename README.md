# MyArs
a simple webapiframework with some tools.\
include autofac,consul,grpc,efcore,identityserver4,redis.\
and some sample with them.

############################################################# 

add Service:

    var builder = WebApplication.CreateBuilder(args);
    builder.Services
    //add arservicecore service
    //nuget[NuGet\Install-Package Ars.Common.Host -Version 1.4.0]
     .AddArserviceCore(builder.Host, config =>
     {
         //add consul client service
         //nuget[NuGet\Install-Package Ars.Common.Consul -Version 1.4.0]
         config.AddArsConsulDiscoverClient();

         //add resource identity service
         //nuget[NuGet\Install-Package Ars.Common.IdentityServer4 -Version 1.4.0]
         config.AddArsIdentityClient();

        //add redis service
        //nuget[NuGet\Install-Package Ars.Common.Redis -Version 1.4.1]
        config.AddArsRedis();

        //add consul register service
        //nuget[NuGet\Install-Package Ars.Common.Consul -Version 1.4.0]
        config.AddArsConsulRegisterServer();

        //add identity server service
        //nuget[NuGet\Install-Package Ars.Common.IdentityServer4 -Version 1.4.0]
        config.AddArsIdentityServer();

        //add localization service
        config.AddArsLocalization();
    })
    //add dbcontext service
    //nuget[NuGet\Install-Package Ars.Common.EFCore -Version 1.4.0]
    .AddArsDbContext<xxxDbContext>();

use Application:

    var app = builder.Build();
    //use ars core application
    app.UseArsCore();
#############################################################

#############################################################

appsettings.Development.json

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
	  //resource server identity config
	  "ArsIdentityClientConfiguration": {
		"Authority": "http://ip:port",
		"ApiName": "apiIds4Second", 
		"RequireHttpsMetadata": false
	  },
	  //redis config
	  "ArsCacheConfiguration": {
		"RedisConnection": "ip",
		"DefaultDB": 1
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
#############################################################

