using System.Diagnostics;

namespace UniKinesisSDK
{
    public static class DebugHelper
    {
        [Conditional("ENABLE_UNIKINESIS_DEBUG")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}