using FluentAssertions;
using MPP_5.DIConfig;
using MPP_5.DIContainer;
using MPP_5.DIExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.HelperClasses;

namespace Tests
{
    public class ExceptionTest
    {
        [Test]
        public void ConstructorExcptnTest()
        {
            Action action = () =>
            {
                DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
                dependenciesConfiguration.Register<IConstructor, Constructor>();
                DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
                dependencyProvider.Resolve<IConstructor>();
            };
            action.Should().Throw<CunstructorNotFoundException>();
        }

        [Test]
        public void DependencyExcptnTest()
        {
            Action action = () =>
            {
                DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
                DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
                dependencyProvider.Resolve<IConstructor>();
            };
            action.Should().Throw<DependencyNotFoundException>();
        }

        [Test]
        public void ImplementionExcptnTest()
        {
            Action action = () =>
            {
                DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
                dependenciesConfiguration.Register<IConstructor, Constructor1>();
                DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
                dependencyProvider.Resolve<IConstructor>();
            };
            action.Should().Throw<ImplementationClassException>();
        }
    }
}
