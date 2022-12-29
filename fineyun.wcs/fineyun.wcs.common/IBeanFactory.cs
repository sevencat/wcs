namespace fineyun.wcs.common;

public interface IBeanFactory
{
	//创建一个再初始化出来
	void InjectProperties(object instance);

	void InjectUnSetProperties(object instance);

	TService CreateInstance<TService>() where TService : class;

	TService Resolve<TService>() where TService : class;

	object Resolve(Type serviceType);

	TService ResolveKeyed<TService>(object serviceKey);

	TService Get<TService>() where TService : class;

	object Get(Type serviceType);

	TService TryGet<TService>() where TService : class;

	TService TryGetKeyed<TService>(object serviceKey) where TService : class;
}