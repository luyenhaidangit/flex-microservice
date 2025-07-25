﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Flex.Core.DependencyInjection
//{
//    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
//    public class ExposeServicesAttribute : Attribute, IExposedServiceTypesProvider
//    {
//        public Type[] ServiceTypes { get; }

//        public bool IncludeDefaults { get; set; }

//        public bool IncludeSelf { get; set; }

//        public ExposeServicesAttribute(params Type[] serviceTypes)
//        {
//            ServiceTypes = serviceTypes ?? Type.EmptyTypes;
//        }

//        public Type[] GetExposedServiceTypes(Type targetType)
//        {
//            var serviceList = ServiceTypes.ToList();

//            if (IncludeDefaults)
//            {
//                foreach (var type in GetDefaultServices(targetType))
//                {
//                    serviceList.AddIfNotContains(type);
//                }

//                if (IncludeSelf)
//                {
//                    serviceList.AddIfNotContains(targetType);
//                }
//            }
//            else if (IncludeSelf)
//            {
//                serviceList.AddIfNotContains(targetType);
//            }

//            return serviceList.ToArray();
//        }

//        private static List<Type> GetDefaultServices(Type type)
//        {
//            var serviceTypes = new List<Type>();

//            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
//            {
//                var interfaceName = interfaceType.Name;
//                if (interfaceType.IsGenericType)
//                {
//                    interfaceName = interfaceType.Name.Left(interfaceType.Name.IndexOf('`'));
//                }

//                if (interfaceName.StartsWith("I"))
//                {
//                    interfaceName = interfaceName.Right(interfaceName.Length - 1);
//                }

//                if (type.Name.EndsWith(interfaceName, StringComparison.OrdinalIgnoreCase))
//                {
//                    serviceTypes.Add(interfaceType);
//                }
//            }

//            return serviceTypes;
//        }
//    }
//}
