using Autofac;
using Autofac.Extensions.DependencyInjection;
using fineyun.wcs.common;
using fineyun.wcs.common.ext;
using fineyun.wcs.support.db;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Persistence.Freesql;

namespace fineyun.wcs.support;

public class SupportModule : Module
{
	private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<BeanFactory>().AsSelf().As<IBeanFactory>().SingleInstance();
		builder.Register((IConfiguration config) => CreateDb(config)).SingleInstance();
		builder.RegisterType<SupportInit>().AsSelf().As<IStartable>().AutoActivate().SingleInstance();
		ConfigWorkFlowCore(builder);
	}

	protected void ConfigWorkFlowCore(ContainerBuilder builder)
	{
		IServiceCollection services = new ServiceCollection();
		services.AddWorkflow(x => x.UseFreeSql(true));
		builder.Populate(services);
	}

	public IFreeSql CreateDb(IConfiguration config)
	{
		var dbconnstr = config["app:db:connstr"];
		if (string.IsNullOrWhiteSpace(dbconnstr))
		{
			Log.Info("未配置数据库");
			throw new Exception("数据库未配置:app:db:connstr");
		}

		Log.Info("开始创建数据库:{0}", dbconnstr);
		var fsql = new FreeSql.FreeSqlBuilder()
			.UseConnectionString(FreeSql.DataType.MySql, dbconnstr)
			.UseAutoSyncStructure(false)
			.Build();
		Log.Info("配置使用jsonmap");
		fsql.UseJsonMap();
		var dbshow_sql = config["app:db:showsql"].ToInt32OrDefault(0);
		if (dbshow_sql == 1)
		{
			Log.Info("已开启fsql调试日志");
			fsql.Aop.CurdBefore += (_, e) => { Log.Info(e.Sql); };
		}

		return fsql;
	}
}