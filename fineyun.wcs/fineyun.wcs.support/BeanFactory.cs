using Autofac;
using fineyun.wcs.common;

namespace fineyun.wcs.support;

public class BeanFactory : IBeanFactory
{
	public BeanFactory(ILifetimeScope c)
	{
		Container = c;
	}

	protected ILifetimeScope Container { get; set; }

	//创建一个再初始化出来
	public void InjectProperties(object instance)
	{
		Container.InjectProperties(instance);
	}

	public void InjectUnSetProperties(object instance)
	{
		Container.InjectUnsetProperties(instance);
	}

	public TService CreateInstance<TService>() where TService : class
	{
		var instance = Activator.CreateInstance<TService>();
		Container.InjectProperties(instance);
		return instance;
	}

	public TService Resolve<TService>() where TService : class
	{
		return Container.Resolve<TService>();
	}

	public object Resolve(Type serviceType)
	{
		return Container.Resolve(serviceType);
	}

	public TService ResolveKeyed<TService>(object serviceKey)
	{
		return Container.ResolveKeyed<TService>(serviceKey);
	}


	public TService Get<TService>() where TService : class
	{
		return Container.Resolve<TService>();
	}

	public object Get(Type serviceType)
	{
		return Container.Resolve(serviceType);
	}

	public TService TryGet<TService>() where TService : class
	{
		return Container.TryResolve<TService>(out var instance) ? instance : null;
	}

	public TService TryGetKeyed<TService>(object serviceKey) where TService : class
	{
		return Container.TryResolveKeyed<TService>(serviceKey, out var instance) ? instance : null;
	}
}