//
// A simple web worker file for the adding/removing/reporting of uploadId 'keep alives'
//
// Expected message format(s):
//
// Add/remove uploadId 'keep alives'...
//  { 
//    "inputData": [
//                   { 
//                    "action": "add" --OR-- "remove"
//                    "uploadId": "..."
//                   }, ...
//                 ]
//  }
//
var uploadIds = []; //Current 'keep alive' uploadIds
var intervalId  = null;

//Handler for incoming messages...
self.onmessage = function (event) {
    var msg = event.data;

    //Validate/initialize input parameters...
    if ('undefined' !== typeof msg && null !== msg &&
        'undefined' !== typeof msg.inputData && null !== msg.inputData) {
        console.log('workerUploadIdKeepAlive - message received...');

        //Input parameters valid - clear the current interval 
        if (null !== intervalId) {
            clearInterval(intervalId);
            intervalId = null;
        }

        //scan inputData array...
        var inputData = msg.inputData;
        var dLength = inputData.length;

        for (dI = 0; dI < dLength; ++dI) {
            var item = inputData[dI];
            var action = item.action.toLowerCase();
            var uploadId = item.uploadId;

            if ('add' === action) {
                //Add uploadId to array...
                uploadIds.push(uploadId);
            }
            else if ('remove' === action) {
                //Remove uploadId from array...
                var index = uploadIds.indexOf(uploadId);
                if (-1 !== index) {
                    uploadIds.splice(index, 1);
                }
            }
        }

        //start a new interval...
        //Use anonymous function here to work in all browsers (including IE 9)
        intervalId = setInterval(function () {
            postUploadIds(uploadIds);
        }, 10000);
    }
}

//Post uploadIds to server - PostCurrentUploadId
function postUploadIds(uploadIds) {
    //Validate/initialize input parameters...
    if ('undefined' !== typeof uploadIds && null !== uploadIds) {
        //Input parameters valid - prepare input uploadIds for post...

        var uploadIdsObject = {
            "uploadIds": uploadIds
        };

        var myUrl = '/api/revisedupload/post/currentuploadid/';
        var xhr = new XMLHttpRequest();

        xhr.open('POST', myUrl, true);   //Asynchronous call...
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.responseType = 'json';      //Specify response type after open call and before send call...

        xhr.onload = function (event) {
            console.log('workerUploadIdKeepAlive - : readyState: ' + xhr.readyState.toString() + ', status: ' + xhr.status.toString());
        };

        xhr.onerror = function (event) {
            console.log('workerUploadIdKeepAlive xhr.onerror: ' + xhr.statusText);
            ////Retun error indicator...
            //self.postMessage({
            //    "requestId": msg.requestId,
            //    "status": "error",
            //    "outputData": { "message": xhr.statusText }
            //});
        };

        xhr.send(JSON.stringify(uploadIdsObject));
    }
}

