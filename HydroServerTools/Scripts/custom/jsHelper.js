
$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: "/Home/GetSyncStatus/",
        success: function (result) {
            
            $('#syncText').text(result);
        },
        error: function (result) {
            $('#syncText').text(result);
        }
    });
});
$("#buttonSyncData").click(function (e) {
    e.preventDefault();
    
    $.ajax({
        type: "POST",
        url: "/Home/RecreateSeriescatalog/",
        success: function (result) {
            alert('ok');
        },
        error: function (result) {
            alert('error');
        }
    });
});