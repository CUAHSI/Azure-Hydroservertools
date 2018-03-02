//
// A simple file of utility methods for use on the 'Revised Uploader' pages

//Request removal of the uploadId...
function removeUploadId(currentUploadId) {
    //Validate/initialize input parameters...
    if ('undefined' !== typeof currentUploadId && null !== currentUploadId) {
        //Input parameters valid - set url...
        var url = '/api/revisedupload/delete/uploadId/' + currentUploadId + '/';

        $.ajax({
            "url": url,
            "type": "DELETE",
            "async": true,
            "dataType": "json",
            "cache": false, //So IE does not cache when calling the same URL - source: http://stackoverflow.com/questions/7846707/ie9-jquery-ajax-call-first-time-doing-well-second-time-not
            "success": function (data, textStatus, jqXHR) {
                console.log('RevisedUpload DELETE/uploadId success!!');

                //Redirect to home page, after a brief delay...
                setTimeout(function () {
                    window.location.href = "/CSVUpload/RevisedUploadData/SelectUploadType"
                }, 1000);
            },
            "error": function (xmlhttprequest, textStatus, message) {
                //Failure - Log messsage received from server...
                console.log('RevisedUpload DELETE/uploadId reports error: ' + xmlhttprequest.status + ' (' + message + ')');
            }
        });
    }
}

