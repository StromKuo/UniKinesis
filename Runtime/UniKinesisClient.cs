namespace UniKinesisSDK
{
    public static class UniKinesisClient
    {
        public static IKinesisClient CreateClient(string kinesisPoolId, string region)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new JSKinesisClient(kinesisPoolId, region);
#else
            return new NativeKinesisClient(kinesisPoolId, region);
#endif
        }
    }
}
