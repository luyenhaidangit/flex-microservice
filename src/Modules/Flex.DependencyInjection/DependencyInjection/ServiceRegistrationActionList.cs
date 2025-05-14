using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Core.DependencyInjection
{
    public class ServiceRegistrationActionList : List<Action<IOnServiceRegistredContext>>
    {
        public bool IsClassInterceptorsDisabled { get; set; }
    }
}
