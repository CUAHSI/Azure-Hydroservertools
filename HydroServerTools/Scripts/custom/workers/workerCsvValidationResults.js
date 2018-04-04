//
// A simple web worker file for the reporting of csv file validation results
//
//Expected message format(s):
//
//  Validation results for all uploaded files...
//  { "requestId": <requestId>,
//    "inputData": {
//                   "action": "get",
//                   "uploadId": "..."
//                 }
//  }
//
//  Validation results for a particular uploaded file...
//  { "requestId": <requestId>,
//    "inputData": { "action": "get",
//                   "uploadId": "...",
//                   "fileName": "..."
//                 }
//  }
//
// Validation Object format: { "<requestId>": { "uploadId": "...",
//                                              "fileNamesToValidationResults": { "<fileName>": {<validation result>},
//                                                                                  ...
//                                                                              }
//                                            }
//                           }
//
var validationObjects = {};     //Current validation objects...

//Handler for incoming messages...
self.onmessage = function (event) {
    var msg = event.data;

    //Validate/initialize input parameters...
    if ('undefined' !== typeof msg && null !== msg &&
        'undefined' !== typeof msg.requestId && null !== msg.requestId &&
        'undefined' !== typeof msg.inputData && null !== msg.inputData) {
        console.log('workerCsvValidationResults - message received: ' + msg.requestId);

        //Input parameters valid - check for input requestId...
        var foundKey = null;
        for (var key in validationObjects) {
            if (key === msg.requestId) {
                foundKey = key;
                break;
            }
        }

        var validationObject = null;
        if (null === foundKey) {
            //Input requestId NOT found - add validation object...
            validationObject = {
                "uploadId": msg.inputData.uploadId,
                "fileNamesToValidationResults": {}
            };

            //Add new object to collection...
            validationObjects[msg.requestId] = validationObject;
        }
        else {
            //Input requestId found - retrieve validation object...
            validationObject = validationObjects[foundKey];
        }

        //Send 'Get' request to server...
        var myUrl = '/api/revisedupload/get/' + msg.inputData.uploadId + '/';
        var xhr = new XMLHttpRequest();

        if ('undefined' !== typeof msg.inputData.fileName && null !== msg.inputData.fileName) {
            //Single file request...
            myUrl = '/api/revisedupload/get/filevalidationresults/' + msg.inputData.uploadId + '/';
            myUrl += msg.inputData.fileName + '/';
        }

        xhr.open('GET', myUrl, true);   //Asynchronous call...
        xhr.responseType = 'json';      //Specify response type after open call and before send call...
        xhr.onload = function (event) {
            console.log('get(validation results) onload: readyState:' + xhr.readyState.toString() + ', status: ' + xhr.status.toString());

            if (4 === xhr.readyState) {     //Transaction complete...
                if (200 === xhr.status) {   //Success...
                    //Initialize result data...
                    var result = {
                        "requestId": msg.requestId,
                        "status": "ok",
                        //"outputData": {
                        //    "uploadId": msg.inputData.uploadId,
                        //    "fileNamesToValidationResults": {}
                        //}
                        "outputData": validationObject
                    };

                    //Get/scan response data...
                    var validationResponse = xhr.response;
                    var length = validationResponse.length;
                    var namesToResults = validationObject.fileNamesToValidationResults;

                    //For each validation results in response...
                    for (var vI = 0; vI < length; ++vI) {
                        var validationResults = validationResponse[vI];

                        var fileName = validationResults.FileName;
                        var fileValidationResults = validationResults.FileValidator;

                        namesToResults[fileName] = fileValidationResults;
                    }

                    //Return result...
                    self.postMessage(result);
                }
            }
        }

        xhr.onerror = function (event) {
            console.log('get(validation results) error: ' + xhr.statusText);
            //Retun error indicator...
            self.PostMessage({
                "requestId": msg.requestId,
                "status": "error",
                "outputData": { "statusText": xhr.statusText }
            });
        };

        xhr.send(null);
    }
};



