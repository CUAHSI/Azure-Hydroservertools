//
// A simple web worker file for the reporting of DB process and load record counts...
//
//Expected message format(s):
//
// Enable/Disable console...
//  { 
//    "inputData": [
//                   { 
//                    "action": "enableDisableConsole"
//                    "value": true --OR-- false
//                   }, ...
//                 ]
//  }
//
//  Record counts for a particular uploaded file...
//  { "requestId": <requestId>,
//    "inputData": { "action": "get",
//                   "uploadId": "...",
//                   "fileName": "..."
//                 }
//  }
//
// Record Count Object format:  { "<requestId>": { "endCondition": true/false,
//                                                 "uploadId": "...",
//                                                 "fileNamesToModelNames": { "<fileName>": "<modelName>",
//                                                                          ...
//                                                                          },
//                                                 "modelNamesToProcessedMessages": { "<modelName>": {"When": ...,
//                                                                                                    "Final": true/false,
//                                                                                                    "Inserted": ...,
//                                                                                                    "Updated": ...,
//                                                                                                    "Rejected": ...,
//                                                                                                    "Duplicated": ...,
//                                                                                                    "RecordCount": ...
//                                                                                                   },
//                                                                                      ...
//                                                                                  }
//
//                                                 "modelNamesToLoadedMessages": { "<modelName>": {"When": ...,
//                                                                                                 "Final": true/false,
//                                                                                                 "Inserted": ...,
//                                                                                                 "Updated": ...,
//                                                                                                 "Rejected": ...,
//                                                                                                 "Duplicated": ...,
//                                                                                                 "RecordCount": ...
//                                                                                                 },
//                                                                                      ...
//                                                                                }
//                                              }
//                              }
//

//Import console utilities...
self.importScripts("/Scripts/custom/ConsoleUtils.js");

var recordCountObjects = {};    //Current record count objects...

//Handler for incoming messages...
self.onmessage = function (event) {
    var msg = event.data;

    //Check for enable/disable console message...
    if ('undefined' !== typeof msg && null !== msg &&
        'undefined' !== typeof msg.inputData && null !== msg.inputData &&
        ('undefined' === typeof msg.requestId || null === msg.requestId)) {
        var inputData = msg.inputData;
        var idLength = inputData.length;

        for (var idI = 0; idI < idLength; ++idI) {
            var msgItem = inputData[idI];

            if ("enabledisableconsole" === msgItem.action.toLowerCase()) {
                //Enable/disable console message found - call console utility...
                EnableDisableConsole(msgItem.value);
                break;
            }
        }
    }

    //Validate/initialize input parameters...
    if ('undefined' !== typeof msg && null !== msg &&
        'undefined' !== typeof msg.requestId && null !== msg.requestId &&
        'undefined' !== typeof msg.inputData && null !== msg.inputData) {
        console.log('workerDbRecordCounts - message received: ' + msg.requestId);

        //Input parameters valid - check for input requestId...
        var foundKey = null;
        for (var key in recordCountObjects) {
            if (key === msg.requestId) {
                foundKey = key;
                break;
            }
        }

        var recordCountObject = null;
        if (null === foundKey) {
            //Input requestId NOT found - add record count object...
            recordCountObject = {
                "endCondition": false,
                "uploadId": msg.inputData.uploadId,
                "fileNamesToModelNames": {},
                "modelNamesToProcessedMessages": {},
                "modelNamesToLoadedMessages": {}
            };

            //Add new object to collection...
            recordCountObjects[msg.requestId] = recordCountObject;

        }
        else {
            //Input requestId found - retrieve record count object...
            recordCountObject = recordCountObjects[foundKey];
        }

        //Send 'Get' request to server...
        //NOTE: Only the filename version of the dbrecordcounts API is currently implemented!! 
        var myUrl = '/api/revisedupload/get/dbrecordcounts/' + msg.inputData.uploadId + '/';
        var xhr = new XMLHttpRequest();

        if ('undefined' !== typeof msg.inputData.fileName && null !== msg.inputData.fileName) {
            //Single file request...
            myUrl = '/api/revisedupload/get/dbrecordcountsforfile/' + msg.inputData.uploadId + '/';
            myUrl += msg.inputData.fileName + '/';
        }

        xhr.open('GET', myUrl, true);   //Asynchronous call...
        xhr.responseType = 'json';      //Specify response type after open call and before send call...
        xhr.onload = function (event) {
            console.log('workerDbRecordCounts - : readyState: ' + xhr.readyState.toString() + ', status: ' + xhr.status.toString());

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
                            "modelNamesToProcessedMessages": {},
                            "modelNamesToLoadedMessages": {}
                        }
                    };

                    //Get response data...
                    var dbRecordCountStatus = xhr.response;
                    if (204 === xhr.status && null === dbRecordCountStatus) {
                        //'No content', 204, received and null response...
                        console.log('workerDbRecordCounts - null === xhr.response!!');
                    }

                    //Check for match with input...
                    if ((null !== dbRecordCountStatus) && (msg.inputData.uploadId === dbRecordCountStatus.uploadId)) {
                        var outputData = result["outputData"];

                        //Load received data, if any, into recordCountObject...
                        recordCountObject.endCondition = outputData.endCondition;

                        //File names to model names...
                        for (var fileName in dbRecordCountStatus.fileNamesToModelNames) {
                            (recordCountObject.fileNamesToModelNames)[fileName] = (dbRecordCountStatus.fileNamesToModelNames)[fileName];
                        }

                        outputData.uploadId = recordCountObject.uploadId;
                        outputData.fileNamesToModelNames = recordCountObject.fileNamesToModelNames

                        //Model names to processed messages...
                        for (var modelName in dbRecordCountStatus.modelNamesToProcessedMessages) {
                            var processedMessage = (dbRecordCountStatus.modelNamesToProcessedMessages)[modelName];

                            (recordCountObject.modelNamesToProcessedMessages)[modelName] = processedMessage;
                            (outputData.modelNamesToProcessedMessages)[modelName] = processedMessage;
                        }

                        //Model names to loaded messages...
                        for (modelName in dbRecordCountStatus.modelNamesToLoadedMessages) {
                            var loadedMessage = (dbRecordCountStatus.modelNamesToLoadedMessages)[modelName];

                            (recordCountObject.modelNamesToLoadedMessages)[modelName] = loadedMessage;

                            (outputData.modelNamesToLoadedMessages)[modelName] = loadedMessage;
                        }

                    }

                    //Return result...
                    self.postMessage(result);
                }
                else {  //NOT success
                    console.log('workerDbRecordCounts - non-successful): ' + xhr.statusText);
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
            console.log('dbrecordcounts xhr.onerror: ' + xhr.statusText);
            //Retun error indicator...
            self.postMessage({
                "requestId": msg.requestId,
                "status": "error",
                "outputData": { "message": xhr.statusText }
            });
        };

        xhr.send(null);
    }
};



