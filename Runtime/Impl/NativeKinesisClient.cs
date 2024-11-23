#if UNITY_EDITOR || !UNITY_WEBGL
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;

namespace UniKinesisSDK
{
    internal class NativeKinesisClient: IKinesisClient
    {
        private IAmazonKinesis _kinesisClient;
        private AWSCredentials _credentials;

        private AWSCredentials Credentials =>
            this._credentials ??= new CognitoAWSCredentials(this._kinesisPoolId,
                RegionEndpoint.GetBySystemName(this._region));

        private IAmazonKinesis Client =>
            this._kinesisClient ??= new AmazonKinesisClient(this.Credentials,
                RegionEndpoint.GetBySystemName(this._region));

        private readonly string _kinesisPoolId;
        private readonly string _region;
        
        private readonly System.Random randomTrace = new System.Random(DateTime.UtcNow.Millisecond);


        public NativeKinesisClient(string kinesisPoolId, string region)
        {
            this._kinesisPoolId = kinesisPoolId;
            this._region = region;
        }

        public void PutRecord(string data, string partitionKey, string streamName)
        {
            if (this.Client == null)
            {
                DebugHelper.LogError(
                    $"PutRecord data: {data}, partitionKey={partitionKey}, KinesisClient is null");
                return;
            }

            DebugHelper.Log($"PutRecord data: {data}, partitionKey={partitionKey}, streamName:{streamName}");

            try
            {
                using var memoryStream = new MemoryStream();
                using var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8, 1024);

                streamWriter.Write(data);
                streamWriter.Flush();

                memoryStream.Position = 0L;

                var putRecordRequest = new PutRecordRequest
                {
                    PartitionKey = partitionKey,
                    StreamName = streamName,
                    Data = memoryStream
                };
                
                DebugHelper.Log($"[PutRecordRequest] PartitionKey: {putRecordRequest.PartitionKey}, StreamName: {putRecordRequest.StreamName}, Data(parameterValue): {data}");

                this.Client.PutRecordAsync(putRecordRequest).ContinueWith(PutRecordCallback);
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"PutRecord(data: {data}, partitionKey={partitionKey}, streamName:{streamName}) Exception: {e.Message}");
            }
        }

        private static void PutRecordCallback(Task<PutRecordResponse> responseObjectTask)
        {
            var responseObject = responseObjectTask.Result;
            DebugHelper.Log(responseObject.HttpStatusCode == System.Net.HttpStatusCode.OK
                ? $"[PutRecordResponse] Successfully put record with SequenceNumber: {responseObject.SequenceNumber}, HttpStatusCode: {responseObject.HttpStatusCode}, ShardId: {responseObject.ShardId}"
                : "Failed put record with Exception");
        }
    }
}
#endif
