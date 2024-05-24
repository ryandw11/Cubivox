using CubivoxCore.Console;

namespace CubivoxClient.Loggers
{
    public class ClientLogger : Logger

    {
        private string modName;
        
        public ClientLogger(string modName)
        {
            this.modName = modName;
        }
        public void Debug(string message)
        {
            UnityEngine.Debug.Log($"[{modName}] DEBUG: " + message);
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError($"[{modName}] ERROR: " + message);
        }

        public void Info(string message)
        {
            UnityEngine.Debug.Log($"[{modName}] INFO: " + message);
        }

        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning($"[{modName}] WARN: " + message);
        }
    }
}