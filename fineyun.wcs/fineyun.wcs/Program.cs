using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CompressedStaticFiles;
using fineyun.wcs.common.ext;
using fineyun.wcs.device;
using fineyun.wcs.http;
using fineyun.wcs.storage;
using fineyun.wcs.support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.FileProviders;
using NJsonSchema.Generation;
using NLog.Extensions.Logging;

namespace fineyun.wcs;

internal class Program
{
	private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

	public static void ConfigIoc(ContainerBuilder builder, ConfigurationManager cfg, IServiceCollection sc)
	{
		builder.RegisterInstance(cfg).As<IConfiguration>();
		builder.RegisterInstance(sc).As<IServiceCollection>();
		builder.RegisterModule(new SupportModule(sc));
		builder.RegisterModule<DeviceModule>();
		builder.RegisterModule<StorageModule>();
	}

	public static void ConfigJsonOpt(JsonOptions x)
	{
		var opts = x.JsonSerializerOptions;
		opts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		opts.PropertyNamingPolicy = null;
		opts.ReferenceHandler = null;
		opts.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
		opts.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	}

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Logging.ClearProviders().AddNLog();

		var config = builder.Configuration;
		var curpath = AppDomain.CurrentDomain.BaseDirectory;
		var wcsConfigFile = Path.Combine(curpath, "wcsconfig.json");
		if (File.Exists(wcsConfigFile))
		{
			Log.Info("增加配置文件:wcsconfig.json");
			config.AddJsonFile(wcsConfigFile);
		}

		var priConfigFile = Path.Combine(curpath, "pri.json");
		if (File.Exists(priConfigFile))
		{
			Log.Info("增加私有配置文件:pri.json");
			config.AddJsonFile(priConfigFile);
		}

		var afsp = new AutofacServiceProviderFactory(x => ConfigIoc(x, config,builder.Services));
		builder.Host.UseServiceProviderFactory(afsp);

		builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
		var mvcBuilder = builder.Services.AddControllersWithViews(opt => opt.Filters.Add<AnnotationsFilter>())
			.AddJsonOptions(ConfigJsonOpt);
		var appParts = mvcBuilder.PartManager.ApplicationParts;
		appParts.Add(new AssemblyPart(typeof(StorageModule).Assembly));
		appParts.Add(new AssemblyPart(typeof(DeviceModule).Assembly));
		appParts.Add(new AssemblyPart(typeof(SupportModule).Assembly));
		appParts.Add(new AssemblyPart(typeof(Program).Assembly));

		mvcBuilder.AddControllersAsServices();

		mvcBuilder.Services.AddCompressedStaticFiles();

		var enableSwagger = config["app:enableswagger"].ToInt32OrDefault(0);
		Log.Info("是否使用swagger:{0}", enableSwagger);
		if (enableSwagger == 1)
		{
			mvcBuilder.Services.AddEndpointsApiExplorer();
			mvcBuilder.Services.AddOpenApiDocument(document =>
			{
				document.Description = "fineyun.wcs";
				document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
			});
		}

		var app = builder.Build();

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			//app.UseHsts();
		}

		{
			var phypath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "www");
			Directory.CreateDirectory(phypath);
			Log.Info("静态资源路径:/www,物理路径:{0}", phypath);
			var opt = new StaticFileOptions
			{
				RequestPath = "/www",
				FileProvider = new PhysicalFileProvider(phypath)
			};
			app.UseCompressedStaticFiles(opt);
		}


		if (enableSwagger == 1)
		{
			app.UseOpenApi();
			app.UseSwaggerUi3();
		}

		app.UseRouting();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		app.Run();
	}
}