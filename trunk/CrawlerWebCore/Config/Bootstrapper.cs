using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace SimpleCrawler
{
    public class Bootstrapper
    {
        private static ContainerBuilder containerBuilder;

        static Bootstrapper()
        {
            containerBuilder = new ContainerBuilder();
        }

        public static void Start()
        {
            containerBuilder.RegisterType<HttpRequestProvider>().As<IHtmlProvider>();
            //containerBuilder.RegisterGeneric(typeof(BusinessBase<>));
            //containerBuilder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>));
            var container = containerBuilder.Build();
            ContainerManager.SetContainer(container);
        }
    }
}
