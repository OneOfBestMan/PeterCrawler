using Autofac;
using System;

namespace SimpleCrawler
{
    public class ContainerManager
    {
     
         private static IContainer _container;
 
         public static void SetContainer(IContainer container)
         {
             _container = container;
         }
 
         public static IContainer Container
         {
             get { return _container; }
         }

         public static T Resolve<T>()
         {
             return _container.Resolve<T>();
         }

        public static object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
         
     }
    
}
