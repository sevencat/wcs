using Autofac;

namespace fineyun.wcs.support;

public class SupportInit : IStartable
{
	private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
	private readonly IFreeSql _fsql;


	public SupportInit(IFreeSql fsql)
	{
		_fsql = fsql;
	}

	public void Start()
	{
		Log.Info("开始初始化");
		var cf = _fsql.CodeFirst;
	}
}