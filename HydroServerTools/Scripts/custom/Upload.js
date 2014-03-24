function myfunc(return_message) {
    eval("data=" + return_message);
    $("#demo_message").html(data.message);
}

$(document).ready(function () {
    //the simple way, use default alert message and callback
    //$("#my_form").jqupload();

    //the second way, use json object and set the message to a div/span
    //$("#my_form").jqupload({"output":"demo_message"});

    //advanced way, define your own logic to handle the return message
    $("#my_form").jqupload({ "callback": "myfunc" });
    $("#my_form").jqupload_form();
    //$('input[type=file]').bootstrapFileInput();

});