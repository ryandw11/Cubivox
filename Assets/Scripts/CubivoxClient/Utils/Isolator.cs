using System;

using CubivoxCore;

namespace CubivoxClient.Utils
{
    public sealed class Isolator
    {
        /// <summary>
        /// Isolates a call stack by catching any exceptions and logging them in the console.
        /// 
        /// This should be used any time "mod" code is called.
        /// </summary>
        /// <param name="action">The action to isolate.</param>
        public static void Isolate(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Cubivox.GetInstance().GetLogger().Error("An internal error has occured!");
                Cubivox.GetInstance().GetLogger().Error(ex.Message);
            }
        }
    }
}