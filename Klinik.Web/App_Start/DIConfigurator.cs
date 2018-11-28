using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.DAL.Interfaces;
using Klinik.DAL.Implementor;

using Unity;
using System.Web.Mvc;
using Klinik.Web.Infrastructure;

namespace Klinik.Web.App_Start
{
    public static class DIConfigurator
    {
        public static void ConfigureDIContainer()
        {
            IUnityContainer container = new UnityContainer();
            registerServices(container);
            DependencyResolver.SetResolver(new DIResolver(container) );
        }

        public static void registerServices(IUnityContainer container)
        {
            container.RegisterType<IEmployee, EmployeeConcrete>();
        }
    }
}