using ApplicationShared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    /// <summary>
    /// This class provides a dummy implementation of the IMail Interface.
    /// It is useful for testing purposes, if you don't want to send emails every time.
    /// </summary>
    public sealed class DummyMailController : IMail
    {
        public Task SendConfirmationMail(string emailAddress)
        {
            return new Task(() => { });
        }

        public Task SendOverbookingMail(string emailAddress)
        {
            return new Task(() => { });
        }
    }
}
