using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineMessenger.MvcServer.ThreadingTools
{
    /// <summary>
    /// Timer of delay execute, execute action after waiting specific time, action can be aborted during this time
    /// </summary>
    public class StopTimer
    {
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private bool IsSkipAction;
        private bool IsActionPerformed;

        private StopTimer(Action<object> action, object obj, int timeout)
        {
            Task.Factory.StartNew(o =>
                                  {
                                      try
                                      {
                                          _resetEvent.WaitOne(timeout);
                                      }
                                      catch (ObjectDisposedException) { }
                                      if (!IsSkipAction)
                                      {
                                          action(o);
                                          IsActionPerformed = true;
                                      }
                                  }, obj);
        }

        /// <summary>
        /// Stops timer if timer not elapsed
        /// </summary>
        public void Stop()
        {
            if (IsSkipAction == false)
            {
                IsSkipAction = true;
                _resetEvent.Set();
                _resetEvent.Dispose();
            }
        }

        /// <summary>
        /// determine if scheduled action was executed
        /// </summary>
        /// <returns></returns>
        public bool ActionExecuted()
        {
            return IsActionPerformed;
        }

        /// <summary>
        /// Run action which will be executed skipping timeout milliseconds
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="obj">Object to pass in action</param>
        /// <param name="timeout">Timeout in end of which action will be executed</param>
        /// <returns></returns>
        public static StopTimer Run(Action<object> action, object obj, int timeout)
        {
            return new StopTimer(action, obj, timeout);
        }
    }
}