//
// A simple web worker file for the reporting of DB load status messages...
//
//Expected message format(s):
//
//  Status messages for all uploaded files...
//  { "requestId": <requestId>,
//    "inputData": {
//                   "action": "get",
//                   "uploadId": "..."
//                 }
//  }
//
//  Status messages for a particular uploaded file...
//  { "requestId": <requestId>,
//    "inputData": { "action": "get",
//                   "uploadId": "...",
//                   "fileName": "..."
//                 }
//  }
//
// Status Object format:  { "<requestId>": { "endCondition": true/false,
//                                           "uploadId": "...",
//                                           "fileNamesToModelNames": { "<fileName>": "<modelName>",
//                                                                          ...
//                                                                    }
//                                           "modelNamesToStatusMessages": { "<modelName>": [{"Message": ...,
//                                                                                            "When": ...
//                                                                                            "Reported": true/false
//                                                                                            "IsError": true/false...
//                                                                                           },... ],
//                                                                          ...
//                                                                         }
//                                         }
//                        }
//
var statusObjects = {};     //Current status objects...

//Handler for incoming messages...
self.onmessage = function (event) {
    var msg = event.data;

    //Validate/initialize input parameters...
    if ('undefined' !== typeof msg && null !== msg &&
        'undefined' !== typeof msg.requestId && null !== msg.requestId &&
        'undefined' !== typeof msg.inputData && null !== msg.inputData) {
        console.log('workerDbLoadStatus - message received: ' + msg.requestId);

        //Input parameters valid - check for input requestId...
        var foundKey = null;
        for (var key in statusObjects) {
            if (key === msg.requestId) {
                foundKey = key;
                break;
            }
        }

        var statusObject = null;
        if (null === foundKey) {
            //Input requestId NOT found - add status object...
            statusObject = { 
                "endCondition": false,
                "uploadId": msg.inputData.uploadId,
                "fileNamesToModelNames": {},
                "modelNamesToStatusMessages": {}
            };

            //Add new object to collection...
            statusObjects[msg.requestId] = statusObject;
        }
        else {
            //Input requestId found - retrieve status object...
            statusObject = statusObjects[foundKey];
        }

        //Send 'Get' request to server...
        var myUrl = '/api/revisedupload/get/dbloadstatus/' + msg.inputData.uploadId + '/';
        var xhr = new XMLHttpRequest();

        if ('undefined' !== typeof msg.inputData.fileName && null !== msg.inputData.fileName) {
            //Single file request...
            myUrl = '/api/revisedupload/get/dbloadstatusforfile/' + msg.inputData.uploadId + '/';
            myUrl += msg.inputData.fileName + '/';
        }

        xhr.open('GET', myUrl, true);   //Asynchronous call...
        xhr.responseType = 'json';      //Specify response type after open call and before send call...
        xhr.onload = function (event) {
            console.log('workerDbLoadStatus - : readyState: ' + xhr.readyState.toString() + ', status: ' + xhr.status.toString());

            if (4 === xhr.readyState) {     //Transaction complete...
                if (200 === xhr.status || 
                    204 === xhr.status ||
                    206 === xhr.status) {   //Success...
                    //Initialize result data...
                    var result = {
                        "requestId": msg.requestId,
                        "status": "ok",
                        "outputData": {
                            "endCondition": (200 === xhr.status) ? true : false,
                            "uploadId": "",
                            "fileNamesToModelNames": {},
                            "modelNamesToStatusMessages": {}
                        }
                    };

                    //Get response data...
                    var dbLoadStatus = xhr.response;    
                    if (204 === xhr.status && null === dbLoadStatus) {
                        //'No content', 204, received and null response...
                        console.log('workerDbLoadStatus - null === xhr.response!!');
                    }

                    //Check for match with input...
                    if ((null !== dbLoadStatus) && (msg.inputData.uploadId === dbLoadStatus.uploadId)) {
                        var outputData = result["outputData"];

                        //Load received data, if any, into statusObject...
                        statusObject.endCondition = outputData.endCondition;

                        //File names to model names...
                        for (var fileName in dbLoadStatus.fileNamesToModelNames) {
                            (statusObject.fileNamesToModelNames)[fileName] = (dbLoadStatus.fileNamesToModelNames)[fileName];
                        }

                        //Model names to status messages...
                        for (var modelName in dbLoadStatus.modelNamesToStatusMessages) {
                            var messages = (dbLoadStatus.modelNamesToStatusMessages)[modelName];
                            var mLength = messages.length;

                            var messageArray = (statusObject.modelNamesToStatusMessages)[modelName];
                            if ('undefined' === typeof messageArray || null === messageArray) {
                                (statusObject.modelNamesToStatusMessages)[modelName] = [];
                                messageArray = (statusObject.modelNamesToStatusMessages)[modelName];
                            }

                            for (var mI = 0; mI < mLength; ++mI) {
                                messageArray.push(messages[mI]);
                            }
                        }

                        //Load unsent message(s), if any, into result...
                        outputData.uploadId = dbLoadStatus.uploadId;
                        outputData.fileNamesToModelNames = statusObject.fileNamesToModelNames;

                        for (modelName in statusObject.modelNamesToStatusMessages) {
                            messages = (statusObject.modelNamesToStatusMessages)[modelName];
                            mLength = messages.length;

                            (outputData.modelNamesToStatusMessages)[modelName] = [];
                            for (mI = 0; mI < mLength; ++mI) {
                                var message = messages[mI];
                                if (!message.Reported) {
                                    message.Reported = true;

                                    (outputData.modelNamesToStatusMessages)[modelName].push(message);
                                }
                            }
                        }
                    }

                    //Return result...
                    self.postMessage(result);
                }
                else {  //NOT success
                    console.log('workerDbLoadStatus - non-successful): ' + xhr.statusText);
                    //Retun error indicator...
                    self.postMessage({
                        "requestId": msg.requestId,
                        "status": "error",
                        "outputData": { "message": xhr.statusText }
                    });                    
                }
            }
        }

        xhr.onerror = function (event) {
            console.log('dbloadstatus xhr.onerror: ' + xhr.statusText);
            //Retun error indicator...
            self.postMessage({
                "requestId": msg.requestId,
                "status": "error",
                "outputData": {"message": xhr.statusText}
            });
        };

        xhr.send(null);
    }
};

