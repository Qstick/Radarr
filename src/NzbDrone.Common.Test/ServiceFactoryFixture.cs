using System.Linq;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Composition.Extensions;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Instrumentation.Extensions;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Datastore.Extensions;
using NzbDrone.Core.Lifecycle;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Test.Common;
using Radarr.Host;

namespace NzbDrone.Common.Test
{
    [TestFixture]
    public class ServiceFactoryFixture : TestBase<ServiceFactory>
    {
        [Test]
        public void event_handlers_should_be_unique()
        {
            var container = new Container(rules => rules.WithNzbDroneRules())
                .AddNzbDroneLogger()
                .AutoAddServices(Bootstrap.ASSEMBLIES)
                .AddDummyDatabase()
                .AddStartupContext(new StartupContext("first", "second"));

            container.RegisterInstance(new Mock<IHostLifetime>().Object);
            container.RegisterInstance(new Mock<IOptions<PostgresOptions>>().Object);

            var serviceProvider = container.GetServiceProvider();

            serviceProvider.GetRequiredService<IAppFolderFactory>().Register();

            Mocker.SetConstant<System.IServiceProvider>(serviceProvider);

            var handlers = Subject.BuildAll<IHandle<ApplicationStartedEvent>>()
                                  .Select(c => c.GetType().FullName);

            handlers.Should().OnlyHaveUniqueItems();
        }
    }
}
