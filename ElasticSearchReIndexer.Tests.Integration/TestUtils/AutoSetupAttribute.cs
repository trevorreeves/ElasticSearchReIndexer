using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils
{
    public class AutoSetupAttribute : AutoDataAttribute
    {
        private const string DefaultFixtureSetupName = "AutoSetup";

        public string[] FixtureSetups { get; set; }

        public AutoSetupAttribute(params string[] fixtureSetups)
            : base(new Fixture())
        {
            if (!fixtureSetups.Any())
            {
                fixtureSetups = new[] { DefaultFixtureSetupName };
            }

            if (!fixtureSetups.Contains(DefaultFixtureSetupName))
            {
                fixtureSetups = new[] { DefaultFixtureSetupName }.Concat(fixtureSetups).ToArray();
            }

            this.FixtureSetups = fixtureSetups;
            this.Fixture.Register(() => this.Fixture);
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            foreach (var action in this.GetSetups(methodUnderTest))
            {
                action(this.Fixture);
            }

            return base.GetData(methodUnderTest, parameterTypes);
        }

        public IEnumerable<Action<IFixture>> GetSetups(MethodInfo method)
        {
            var setupActions = new List<Action<IFixture>>();
            var type = method.DeclaringType;

            foreach (var fixtureSetup in this.FixtureSetups.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var setups =
                    this.GetActionsFromProperty(type, fixtureSetup).Concat(
                    this.GetActionsFromMethods(type, fixtureSetup)).Concat(
                    this.GetActionsFromField(type, fixtureSetup))
                    .ToList();

                if (!setups.Any() && !fixtureSetup.Equals(DefaultFixtureSetupName, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentOutOfRangeException(fixtureSetup, "No static property, method or field could be found on the test fixture with the name " + fixtureSetup);
                }

                setupActions.AddRange(setups);
            }

            return setupActions;
        }

        private IEnumerable<Action<IFixture>> GetActionsFromProperty(Type type, string fixtureAction)
        {
            var property = type.GetProperty(
                fixtureAction,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (property == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var obj = property.GetValue(null);
            try
            {
                return this.ParseFixtureActionValue(obj);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Property {0} on {1} did not return IEnumerableAction<IFixture>> or Action<IFixture>",
                        fixtureAction,
                        type.FullName));
            }
        }

        private IEnumerable<Action<IFixture>> GetActionsFromMethods(Type type, string fixtureAction)
        {
            var method = type.GetMethod(
                fixtureAction,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (method == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var obj = method.Invoke(null, new object[] { });

            try
            {
                return this.ParseFixtureActionValue(obj);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Method {0} on {1} did not return IEnumerableAction<IFixture>> or Action<IFixture>",
                        fixtureAction,
                        type.FullName));
            }
        }

        private IEnumerable<Action<IFixture>> GetActionsFromField(Type type, string fixtureAction)
        {
            var field = type.GetField(
                fixtureAction,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var obj = field.GetValue(null);

            try
            {
                return this.ParseFixtureActionValue(obj);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Field {0} on {1} did not return IEnumerableAction<IFixture>> or Action<IFixture>",
                        fixtureAction,
                        type.FullName));
            }
        }

        private IEnumerable<Action<IFixture>> ParseFixtureActionValue(object obj)
        {
            if (obj == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var enumerable = obj as IEnumerable<Action<IFixture>>;
            if (enumerable == null)
            {
                var single = obj as Action<IFixture>;
                if (single == null)
                {
                    throw new ArgumentNullException();
                }
                enumerable = new List<Action<IFixture>> { single };
            }

            return enumerable;
        }
    }
}
