namespace UniKinesisSDK
{
    public interface IKinesisClient
    {
        public void PutRecord(string data, string partitionKey, string streamName);
    }
}