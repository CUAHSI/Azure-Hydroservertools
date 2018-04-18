
// A simple JavaScript 'class' for the encapsulation of web worker monitoring
//
"use strict"

function WorkerMonitor() {
    //Check for use of 'new' in instantiation...
    if (this instanceof WorkerMonitor) {
        //Instance created with new 

        //Initialize member variables...
        this.promiseResolves = {};
        this.promiseRejects = {};

        this.monitoredWorker = null;

        //Prototype methods...

        //Start monitoring
        WorkerMonitor.prototype.startMonitoring = function (monitoredWorker) {

            //Validate/initialize input parameters...
            if ('undefined' !== typeof monitoredWorker && null !== monitoredWorker &&
                monitoredWorker instanceof Worker) {

                //Input parameters valid - reset current instance...
                this.stopMonitoring();

                //Assign worker and message handler...
                this.monitoredWorker = monitoredWorker;
                //Must bind current instance to receive function to preserve correct value of 'this' 
                // during worker 'onmessage' call...
                var myBoundFunction = this.receiveWorkerMessage.bind(this);
                this.monitoredWorker.onmessage = myBoundFunction;
            }
        };

        //Stop monitoring
        WorkerMonitor.prototype.stopMonitoring = function () {

            //reset all members...
            this.promiseResolves = {};
            this.promiseRejects = {};

            if (null !== this.monitoredWorker) {
                this.monitoredWorker.onmessage = null;
                this.monitoredWorker = null;
            }
        };

        //Send message - for the currently monitored worker, if any, 
        // post a message (in a format known to the worker)
        //
        //Expected message format:
        //  { "requestId": <requestId>,
        //    "inputData": {
        //                   <data to be consumed by worker>
        //                 }
        //  }
        //
        // Return: For an unknown request Id: 
        //          - a Promise ('to-be-resolved/rejected' per the worker's response to the posted message)
        //         For an known request Id:
        //          - null (indicating the request Id has a 'still outstanding' associated Promise)
        WorkerMonitor.prototype.sendWorkerMessage = function (workerMessage) {
            var result = null;

            if (null !== this.monitoredWorker) {
                //Monitored worker exists - validate/initialize input parameters...
                if ('undefined' !== typeof workerMessage && null !== workerMessage &&
                    'undefined' !== typeof workerMessage.requestId && null !== workerMessage.requestId &&
                    'undefined' !== typeof workerMessage.inputData && null !== workerMessage.inputData ) {
                    //Input parameters valid - check for outstanding promise on input requestId...
                    var inputRequestId = workerMessage.requestId;
                    var bFound = false;
                    for (var requestId in this.promiseResolves) {
                        if (requestId === inputRequestId) {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound) {
                        //No outstanding promise found - create a new promise...
                        var myPromiseResolves = this.promiseResolves;
                        var myPromiseRejects = this.promiseRejects;
                        var myMonitoredWorker = this.monitoredWorker;

                        result = new Promise(function (resolve, reject) {
                            //Retain resolve and reject for later reference...
                            myPromiseResolves[inputRequestId] = resolve;
                            myPromiseRejects[inputRequestId] = reject;

                            myMonitoredWorker.postMessage(workerMessage);
                        });
                    }
                }
            }

            //Processing complete - return result...
            return result;
        };

        //Send message - for the currently monitored worker, if any, 
        // post a message (in a format known to the worker)
        //
        //Expected message format:
        //  { ["requestId": <requestId>,]   //Optional - not referenced
        //    "inputData": {
        //                   <data to be consumed by worker>
        //                 }
        //  }
        //
        // Always return a null - never return a Promise... 
        WorkerMonitor.prototype.simpleSendWorkerMessage = function (workerMessage) {
            var result = null;

            if (null !== this.monitoredWorker) {
                //Monitored worker exists - validate/initialize input parameters...
                if ('undefined' !== typeof workerMessage && null !== workerMessage &&
                    'undefined' !== typeof workerMessage.inputData && null !== workerMessage.inputData) {

                    var myMonitoredWorker = this.monitoredWorker;

                    myMonitoredWorker.postMessage(workerMessage);
                }
            }

            //Processing complete - return result...
            return result;
        };

        //Retrieve message - for the currently monitored worker, 
        // receive a message in a format known to the caller.
        //
        //Expected message format:
        //  { "requestId": <requestId>,
        //    "status": <'ok' --OR-- 'error'>
        //    "outputData": {
        //                    <data to be consumed by caller >
        //                  }
        //  }
        //
        // If successful ('status': 'ok') resolve the associated promise 
        // else reject the associated promise
        WorkerMonitor.prototype.receiveWorkerMessage = function (event) {

            if (null !== this.monitoredWorker) {
                //Monitored worker exists - validate/initialize input parameters...
                var workerMessage = event.data;
                if ('undefined' !== typeof workerMessage && null !== workerMessage &&
                    'undefined' !== typeof workerMessage.requestId && null !== workerMessage.requestId &&
                    'undefined' !== typeof workerMessage.status && null !== workerMessage.status &&
                    'undefined' !== typeof workerMessage.outputData && null !== workerMessage.outputData) {
                    //Input parameters valid - check status...
                    var requestId = workerMessage.requestId;
                    var status = workerMessage.status;
                    console.log('Message received: ' + requestId + ' , status: ' + status);

                    var bFound = false;
                    if ('ok' === status.toLowerCase()) {
                        //Success - find associated 'resolve', if any...
                        for (var resolveKey in this.promiseResolves) {
                            if (requestId === resolveKey) {
                                //'Resolve' found - call, if indicated...
                                bFound = true;
                                var resolve = this.promiseResolves[requestId];
                                if (resolve) {
                                    resolve(workerMessage);
                                }
                                break;
                            }
                        }
                    }
                    else if ('error' === status.toLowerCase()) {
                        //Error - find associated 'reject', if any...
                        for (var rejectKey in this.promiseRejects) {
                            if (requestId === rejectKey) {
                                //'Reject' found - call, if indicated...
                                bFound = true;
                                var reject = this.promiseRejects[requestId];
                                if (reject) {
                                    reject(workerMessage);
                                }
                                break;
                            }
                        }
                    }

                    if (bFound) {
                        //'Resolve' --OR-- 'Reject' found - delete both...
                        delete this.promiseResolves[requestId];
                        delete this.promiseRejects[requestId];
                    }
                }
            }
        };

    }
    else {
        //Instance NOT created with new - throw exception
        throw new Error("ALWAYS use 'new' to create an instance...");
    }
}