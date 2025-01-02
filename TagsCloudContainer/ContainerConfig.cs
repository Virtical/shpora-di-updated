using System.Drawing;
using Autofac;

namespace TagsCloudContainer;

public static class ContainerConfig
{
    public static IContainer Configure(Point center)
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<ArchimedeanSpiral>()
            .As<ISpiral>()
            .WithParameter("center", center);

        builder.RegisterType<CircularCloudLayouter>()
            .AsSelf()
            .WithParameter("center", center)
            .PropertiesAutowired();

        return builder.Build();
    }
}