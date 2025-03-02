using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Contracts.Services
{
    public interface ISmtpEmailService : IEmailService<MailRequest>
    {
    }
}
