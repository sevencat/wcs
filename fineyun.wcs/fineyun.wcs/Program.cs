using CompressedStaticFiles;
using fineyun.wcs.common.ext;
using Microsoft.Extensions.FileProviders;
using NJsonSchema.Generation;
using NLog.Extensions.Logging;

namespace fineyun.wcs;

internal class Program
{
	private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Logging.ClearProviders().AddNLog();
		var config = builder.Configuration;

		var mvcBuilder = builder.Services.AddControllersWithViews();
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