
//$(document).ready(function () {
//    $.ajax({
//        type: "POST",
//        url: "/Home/GetSyncStatus/",
//        success: function (result) {
            
//            $('#syncText').text(result);
//        },
//        error: function (result) {
//            $('#syncText').text(result);
//        }
//    });
//});
$("#RequestPublication").click(function (e) {
    e.preventDefault();
    
    $.ajax({
        type: "POST",
        url: "/Home/RequestPublication/",
        success: function (result) {
            alert('Request submitted successfully to help@cuahsi.org ');
        },
        error: function (result) {
            alert('Request Submission failed. Please contact help@cuahsi.org');
        }
    });
});