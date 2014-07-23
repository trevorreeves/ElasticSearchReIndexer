using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.Windsor;

namespace treeves.essentials.castle.windsor
{
    public static class WindsorContainerExtensions
    {
        public static IWindsorContainer DependsOnFacility<TFacility>(
            this IWindsorContainer container)
            where TFacility : IFacility, new()
        {
            if (!container.HasFacility<TFacility>())
            {
                container.AddFacility<TFacility>();
            }

            return container;
        }

        public static IWindsorContainer DependsOnFacility<TFacility>(
            this IWindsorContainer container,
            Action<TFacility> onCreate)
            where TFacility : IFacility, new()
        {
            if (!container.HasFacility<TFacility>())
            {
                container.AddFacility(onCreate);
            }

            return container;
        }

        public static bool HasFacility<TFacility>(this IWindsorContainer container) where TFacility : IFacility, new()
        {
            return container.Kernel.GetFacilities()
                .Any(facility => facility.GetType().IsAssignableFrom(typeof(TFacility)));
        }
    }
}
