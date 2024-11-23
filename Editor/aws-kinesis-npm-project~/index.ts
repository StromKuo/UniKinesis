import { KinesisClient, PutRecordCommand } from "@aws-sdk/client-kinesis";
import { CognitoIdentityClient } from "@aws-sdk/client-cognito-identity";
import { fromCognitoIdentityPool } from "@aws-sdk/credential-provider-cognito-identity";

// Declare global variables
declare global {
    interface Window {
        UniKinesis_Initialize: typeof UniKinesis_Initialize;
        UniKinesis_PutRecord: typeof UniKinesis_PutRecord;
    }

    // Declare Module variable (Emscripten module object in Unity WebGL)
    var Module: any;
}

// Global Kinesis client instance
let kinesisClient: KinesisClient | null = null;

// Initialization function
function UniKinesis_Initialize(identityPoolId: string, region: string): void {

    // Create Kinesis client
    kinesisClient = new KinesisClient({
        region: region,
        credentials: fromCognitoIdentityPool({
            client: new CognitoIdentityClient({ region: region }),
            identityPoolId: identityPoolId,
        }),
    });

    console.log('AWS Kinesis client initialized');
}

// Function to send record to Kinesis
async function UniKinesis_PutRecord(dataStr: string, partitionKey: string, streamName: string): Promise<void> {
    if (!kinesisClient) {
        console.error('Kinesis client not initialized, please call UniKinesis_Initialize first');
        return;
    }

    const data = new TextEncoder().encode(dataStr);

    const command = new PutRecordCommand({
        Data: data,
        PartitionKey: partitionKey,
        StreamName: streamName,
    });

    try {
        const response = await kinesisClient.send(command);
        console.log("PutRecord successful:", response);
    } catch (error) {
        console.error("Error sending record to Kinesis:", error);
    }
}

// Attach functions to the window object
window.UniKinesis_Initialize = UniKinesis_Initialize;
window.UniKinesis_PutRecord = UniKinesis_PutRecord;
