#if UNITY_WEBGL // && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace UniKinesisSDK
{
    internal class JSKinesisClient : IKinesisClient
    {
        [DllImport("__Internal")]
        private static extern void UniKinesis_Initialize_Bridge(string identityPoolId, string region);

        [DllImport("__Internal")]
        private static extern void UniKinesis_PutRecord_Bridge(string data, string partitionKey, string streamName);

        private readonly System.Random randomTrace = new System.Random(DateTime.UtcNow.Millisecond);

        public JSKinesisClient(string kinesisPoolId, string region)
        {
            UniKinesis_Initialize_Bridge(kinesisPoolId, region);
        }

        public void PutRecord(string data, string partitionKey, string streamName)
        {
            DebugHelper.Log(
                $"PutRecord data: {data}, partitionKey: {partitionKey}, streamName: {streamName}");
            UniKinesis_PutRecord_Bridge(data, partitionKey, streamName);
        }
    }
}
#endif