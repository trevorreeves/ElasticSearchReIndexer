using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils
{
    public interface IAutoTestInstance
    {
        IAutoTestInstance Arrange(params Action<IFixture>[] customizations);

        IAutoTestInstance Start<T>(Action<T> testMethod);

        IAutoTestInstance Start<T1, T2>(Action<T1, T2> testMethod);

        IAutoTestInstance Start<T1, T2, T3>(Action<T1, T2, T3> testMethod);

        IAutoTestInstance Start<T1, T2, T3, T4>(Action<T1, T2, T3, T4> testMethod);

        IAutoTestInstance Start<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> testMethod);

        IAutoTestInstance Start<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> testMethod);

        IAutoTestInstance Start<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> testMethod);
    }

    public class AutoTestInstance : IAutoTestInstance
    {
        private readonly IFixture _fixture;

        public AutoTestInstance() :
            this(new Fixture())
        {
        }

        public AutoTestInstance(IFixture fixture)
        {
            _fixture = fixture;
            _fixture.Register<IFixture>(() => _fixture);
        }

        public IAutoTestInstance Arrange(params Action<IFixture>[] customizations)
        {
            foreach (Action<IFixture> customization in customizations)
            {
                customization(_fixture);
            }

            return this;
        }

        public IAutoTestInstance Start<T>(Action<T> testMethod)
        {
            testMethod(_fixture.Create<T>());
            return this;
        }

        public IAutoTestInstance Start<T1, T2>(Action<T1, T2> testMethod)
        {
            testMethod(_fixture.Create<T1>(), _fixture.Create<T2>());
            return this;
        }

        public IAutoTestInstance Start<T1, T2, T3>(Action<T1, T2, T3> testMethod)
        {
            testMethod(_fixture.Create<T1>(), _fixture.Create<T2>(), _fixture.Create<T3>());
            return this;
        }

        public IAutoTestInstance Start<T1, T2, T3, T4>(Action<T1, T2, T3, T4> testMethod)
        {
            testMethod(
                _fixture.Create<T1>(), 
                _fixture.Create<T2>(),
                _fixture.Create<T3>(), 
                _fixture.Create<T4>());
            return this;
        }

        public IAutoTestInstance Start<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> testMethod)
        {
            testMethod(
                _fixture.Create<T1>(),
                _fixture.Create<T2>(),
                _fixture.Create<T3>(),
                _fixture.Create<T4>(),
                _fixture.Create<T5>());
            return this;
        }

        public IAutoTestInstance Start<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> testMethod)
        {
            testMethod(
                _fixture.Create<T1>(),
                _fixture.Create<T2>(),
                _fixture.Create<T3>(),
                _fixture.Create<T4>(),
                _fixture.Create<T5>(),
                _fixture.Create<T6>());

            return this;
        }

        public IAutoTestInstance Start<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> testMethod)
        {
            testMethod(
                _fixture.Create<T1>(),
                _fixture.Create<T2>(),
                _fixture.Create<T3>(),
                _fixture.Create<T4>(),
                _fixture.Create<T5>(),
                _fixture.Create<T6>(),
                _fixture.Create<T7>());

            return this;
        }
    }

    public static class AutoTest
    {
        public static IAutoTestInstance With(IFixture fixture)
        {
            return new AutoTestInstance(fixture);
        }

        public static IAutoTestInstance Arrange(params Action<IFixture>[] customizations)
        {
            var instance = new AutoTestInstance();
            instance.Arrange(customizations);
            return instance;
        }
    }

    public static class Apply
    {
        public static Action<IFixture> Customization<T>()
            where T : ICustomization, new()
        {
            return (fixture) =>
            {
                T obj = Activator.CreateInstance<T>();
                obj.Customize(fixture);
            };
        }

        public static Action<IFixture> Method<T>(string staticMethodName)
        {
            return (fixture) =>
            {

            };
        }
    }
}
