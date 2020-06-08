using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationShared
{
    public interface IMail
    {
        Task SendConfirmationMail(string emailAddress);
        Task SendOverbookingMail(string emailAddress);
    }
}
