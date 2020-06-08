using ApplicationShared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class MailController : IMail
    {
        public Task SendConfirmationMail(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task SendOverbookingMail(string emailAddress)
        {
            throw new NotImplementedException();
        }
    }
}
