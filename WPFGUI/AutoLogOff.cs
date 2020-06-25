using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Application;
using ApplicationShared;
using Core;
using WPFGUI.ViewModels;

namespace WPFGUI
{
    class AutoLogOff
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        static CancellationTokenSource source = new CancellationTokenSource();
        static CancellationToken token = source.Token;

        public static CancellationTokenSource GetToken => source;

        public static IdleTimeInfo GetIdleTimeInfo()
        {
            int systemUptime = Environment.TickCount,
                lastInputTicks = 0,
                idleTicks = 0;

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                lastInputTicks = (int)lastInputInfo.dwTime;

                idleTicks = systemUptime - lastInputTicks;
            }

            return new IdleTimeInfo
            {
                LastInputTime = DateTime.Now.AddMilliseconds(-1 * idleTicks),
                IdleTime = new TimeSpan(0, 0, 0, 0, idleTicks),
                SystemUptimeMilliseconds = systemUptime,
            };
        }

        public static void CreateLogoutThread(NavigationViewModel _navigationViewModel, User user, int logoutTime)
        {
            Task task = new Task(() => CheckUsage(_navigationViewModel, user, logoutTime, token));
            task.Start();
        }

        public static async void CheckUsage(NavigationViewModel _navigationViewModel, User user, int logoutTime, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    //close
                    return;
                }

                var idleTime = GetIdleTimeInfo();

                if (idleTime.IdleTime.TotalMinutes >= logoutTime)
                {
                    IUser _user = new UserController();
                    var logout = await _user.Logout(user.Username);
                    if (logout)
                    {
                        string info = "Sie wurden wegen Inaktivität automatisch Ausgeloggt.";
                        _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel, info);
                    }
                    return;
                }
                Thread.Sleep(1000);
            }
        }
    }

    public class IdleTimeInfo
    {
        public DateTime LastInputTime { get; internal set; }

        public TimeSpan IdleTime { get; internal set; }

        public int SystemUptimeMilliseconds { get; internal set; }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}
