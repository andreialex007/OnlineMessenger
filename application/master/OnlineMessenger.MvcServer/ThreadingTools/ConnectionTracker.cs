using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.Tools;

namespace OnlineMessenger.MvcServer.ThreadingTools
{
    /// <summary>
    /// Tracks existing connections and rises disconnection events
    /// </summary>
    public class ConnectionTracker
    {
        /// <summary>
        /// Disconnection time between requests
        /// </summary>
        private const int DisconnectTimeout = 2000;

        /// <summary>
        /// Stop timers which allows wait time during connections which helps do not disconnect user every time
        /// </summary>
        private static readonly Dictionary<string, StopTimer> StopTimers = new Dictionary<string, StopTimer>();

        /// <summary>
        /// Write to db information about disconnected user ( his state ) and notify all connected users
        /// </summary>
        /// <param name="userName">User name to disconnect</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ProcessDisconnection(string userName)
        {
            if (String.IsNullOrEmpty(userName))
                return;

            RemoveStopTimerIfExists(userName);
            StopTimers.Add(userName, StopTimer.Run(o => ExecuteDisconnectNotifications(o as string), userName, DisconnectTimeout));
        }

        /// <summary>
        /// Write to db information about disconnected user ( his state ) and notify all connected users
        /// </summary>
        /// <param name="userName">User name to disconnect</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ProcessConnection(string userName)
        {
            if (String.IsNullOrEmpty(userName))
                return;

            if (StopTimers.ContainsKey(userName))
            {
                if (StopTimers[userName].ActionExecuted())
                    ExecuteConnectNotifications(userName);

                StopTimers[userName].Stop();
                StopTimers.Remove(userName);
            }
            else
            {
                ExecuteConnectNotifications(userName);
            }
        }


        #region private methods

        private static void ExecuteConnectNotifications(string userName)
        {
            if (String.IsNullOrEmpty(userName))
                return;

            var messageServer = Bootstrapper.Resolve<IMessageServer>();
            messageServer.SetConnected(userName, true);
            messageServer.UserConnected(userName);
        }


        private static void ExecuteDisconnectNotifications(string userName)
        {
            if (String.IsNullOrEmpty(userName))
                return;

            var messageServer = Bootstrapper.Resolve<IMessageServer>();
            messageServer.SetConnected(userName, false);
            messageServer.UserDisconnected(userName);
        }

        private static void RemoveStopTimerIfExists(string userName)
        {
            if (StopTimers.ContainsKey(userName))
            {
                StopTimers[userName].Stop();
                StopTimers.Remove(userName);
            }
        }

        #endregion
    }
}