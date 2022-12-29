using Autofac;
using fineyun.wcs.common;

namespace fineyun.wcs.support;

public class SupportModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<BeanFactory>().AsSelf().As<IBeanFactory>().SingleInstance();
	}
}