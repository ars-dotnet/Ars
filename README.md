<p align="center">
  <img height="140" src="https://github.com/aabb1212/Ars/blob/master/logo.png">
</p>

# Ars
[![NuGet](https://img.shields.io/nuget/v/Ars.Common.Host.svg)](https://www.nuget.org/packages/Ars.Common.Host/)

a simple .net6 webapi framework with some extensions.\
include autofac,consul,grpc,efcore,identityserver4,redis,signalr,localization,skyapm,upload and download excel,cap extensions. \
and some samples with them.

## Getting Started

### NuGet

ars can be installed in your project with the following command.

```
PM> Install-Package Ars.Common.Host
```

ars supports some extensions, following packages are available to install:

```
PM> Install-Package Ars.Common.IdentityServer4
PM> Install-Package Ars.Common.Consul
PM> Install-Package Ars.Common.EFCore
PM> Install-Package Ars.Common.Redis
PM> Install-Package Ars.Common.SignalR
PM> Install-Package Ars.Common.SkyWalking
PM> Install-Package Ars.Common.Cap
```

### Configuration
#### add Service:

```
var builder = WebApplication.CreateBuilder(args);
builder.Services
    //add ars core service
    .AddArserviceCore(builder, config =>
    {
        //add consul client service
        config.AddArsConsulDiscoverClient();
	 
        //add consul register service
        config.AddArsConsulRegisterServer();

        //add identity resource service
        config.AddArsIdentityClient();
	 
        //add identity server service
        config.AddArsIdentityServer();

        //add redis service
        config.AddArsRedis();

        //add localization service
        config.AddArsLocalization();

        //add signalr service
        config.AddArsSignalR(config =>
        {
		config.CacheType = 0;
		config.UseMessagePackProtocol = true;
        });

	//add skyapm service
	config.AddArsSkyApm();

	//add cap service
	config.AddArsCap(option => 
	{
		option.UseEntityFramework<xxxDbContext>();
		option.UseRabbitMQ(mq => 
		{
			mq.HostName = "localhost";
			mq.UserName = "guest";
			mq.Password = "guest";
		});
	});

	//add ocelot service
	//example service -> sample/main/ArsApiGateway
	config.AddArsOcelot(option => 
        {
	   //如果下游协议是https则添加下面代码
	   //option.AddDelegatingHandler<X509CertificateDelegatingHandler>();
        });
})

//add dbcontext service
.AddArsDbContext<xxxDbContext>()

//add exportexcel service
.AddArsExportExcelService(typeof(Program).Assembly)

//add uploadexcel service
.AddArsUploadExcelService(option =>
{
	option.UploadRoot = "wwwroot/upload";
	option.RequestPath = "apps/upload";
	option.SlidingExpireTime = TimeSpan.FromDays(1);
});
```

#### use Application:

```
    var app = builder.Build();
    
    //ars exception middleware
    app.UsArsExceptionMiddleware();

	.....

    //use ars core application
    app.UseArsCore() 
    //use uploadexcel application
    .UseArsUploadExcel();
```

#### change your appsettings.Development.json
 ```
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
			"Communication":{
			    "CommunicationWay":0,
				"GrpcUseHttp1Protocol": true,
				"UseHttps": true,
				"UseIdentityServer4Valid": true,
				"IdentityServer4Address": "http://ip:port",
				"ClientId": "grpc-key",
				"ClientSecret": "grpc-secret",
				"Scope": "grpcapi-scope",
				"GrantType": "client_credentials"
			}
		  }
		]
	  },
	  
	  //consul register config
	  "ConsulRegisterConfiguration": {
		"ConsulAddress": "http://ip:port",
		"ServiceName": "apigrpc",
		"UseHttps": true,
	  },
	  
	  //resource server identity config
	  "ArsIdentityClientConfiguration": {
		"Authority": "http://ip:port",
		"ApiName": "apiIds4Second", 
		"RequireHttpsMetadata": true,
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
		"DefaultString": "Data Source=xxxxx; Initial Catalog=xxxxx;user id=xxxxx;pwd=xxxxx"
	  },

	  //Basic config
	  "ArsBasicConfiguration": {
		 "ServiceIp": "192.168.110.65",
		 "ServicePort": 5105,
		 "CertificatePath": "Certificates//IS4.pfx",
		 "CertificatePassWord": "aabb1212",
		 "UseHttps": true
	  }
	}
```

### License

[MIT](https://github.com/aabb1212/Ars/blob/master/LICENSE.md)
