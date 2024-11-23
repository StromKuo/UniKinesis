mergeInto(LibraryManager.library, {
    UniKinesis_Initialize_Bridge: function(identityPoolId, region) {
        var identityPoolIdStr = UTF8ToString(identityPoolId);
        var regionStr = UTF8ToString(region);

        UniKinesis_Initialize(identityPoolIdStr, regionStr);
    },

    UniKinesis_PutRecord_Bridge: function(dataPtr, partitionKeyPtr, streamNamePtr) {
        var dataStr = UTF8ToString(dataPtr);
        var partitionKey = UTF8ToString(partitionKeyPtr);
        var streamName = UTF8ToString(streamNamePtr);

        UniKinesis_PutRecord(dataStr, partitionKey, streamName);
    }
});