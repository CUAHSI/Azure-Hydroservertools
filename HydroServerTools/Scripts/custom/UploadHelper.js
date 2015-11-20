//$(document).ready(function () {
//    $('input[type=file]').bootstrapFileInput();
//});

//$(function () {

    //var viewName = $.url().param('identifier');

    // $('form').append('<input type="hidden" name="identifier" value="' + viewName + '">')

    var sPath = window.location.pathname;
    var viewName = sPath.substring(sPath.lastIndexOf('/') + 1);
    var intervalId;
    //var acceptFiletypes= ["zip","csv"];
    //$('#refresh').bind("click");
    //$('#refresh').click(function ()
    //{
    //    alert()
    //})
        
    $('#fileupload').fileupload({
        dataType: "json",
        url: "/api/upload/" + viewName,
        limitConcurrentUploads: 1,
        sequentialUploads: true,
        acceptFileTypes: /(\.|\/)(zip|csv)$/i,
        replaceFileInput: false,
        

        add: function (e, data) {

            var filename = data.files[0].name;

            if (!checkFileType(filename)) { alert("This filetype is not allowed"); return false; }

            if ($('#filelistholder').text().length > 0) {
                //reset not very elegant...  
                $('#filelistholder').text("");
                //remove click event to prevent submitting additional files
                $('#startupload').unbind('click')
            }

            $('#startupload').removeClass('disabled');
            $('#reset').removeClass('disabled');

            $('#filelistholder').removeClass('hide');
            data.context = $('<div />').text('Selected file: ' + data.files[0].name).appendTo('#filelistholder');
            //$('</div><div class="progress"><div class="bar" style="width:0%"></div></div>').appendTo(data.context);
            $('#startupload').click(function () {
                
                // intervalId = setInterval(setupinterval, 1000)
                $('.fileinput-button').addClass('disabled');
                $('#startupload').addClass('disabled');
                $('#reset').addClass('disabled');
                $('#loading').removeClass('hide');
                $('#reset').unbind('click')
              

                intervalId = interval(function () {
                    funcName();
                }, 2000, 1000);

                data.submit();
               
                

                //kick off the process
                //start polling
                //(function poll() {
                //    setTimeout(function () {
                //        $.Ajax({
                //            url: "/Home/Progress",
                //            success: function (data) {
                //                //Update the progress bar
                //                //setProgress(data.value);
                //                alert();
                //                //Setup the next poll recursively
                //                poll();
                //            },
                //            fail: function ()
                //            { alert("fail")},
                //            dataType: "json"
                //        });
                //    }, 1000);
                //})();




                // intervalId = setInterval(funcName,1000)
                //var i = 0;
                //(function foo() {
                //    $.post("/Home/Progress", function (progress) {
                //        //if (progress >= 1000) {
                //        //    updateMonitor(taskId, "Completed");
                //        //    clearInterval(intervalId);
                //        //} else {
                //        updateMonitor(status, progress);
                //        //}
                //    });
                //    setTimeout(foo, 1000);
                //})();


                //intervalId = setInterval(function () {

                //    $.post("/Home/Progress", function (progress) {
                //        //if (progress >= 1000) {
                //        //    updateMonitor(taskId, "Completed");
                //        //    clearInterval(intervalId);
                //        //} else {
                //        updateMonitor(status, progress);
                //        //}
                //    });

                //}, 100);
               
            });
             
        },
        done: function (data, o) {
            //data.context.text(data.files[0].SiteID + '... Completed');
            clearInterval(intervalId);
            //updateMonitor("Done", "Processing completed");
            window.location.href = '/CSVUpload/' +viewName
            //alert(result);
            //$('</div><div class="progress"><div class="bar" style="width:60%"></div></div>').appendTo(data.context);
        },
        //progressall: function (e, data) {
        //    var progress = parseInt(data.loaded / data.total * 100, 10);
        //    //$('#overallbar').css('width', progress + '%');

        //    $("#progressbar").progressbar({
        //        value: progress
        //    });
        //},
        //progress: function (e, data) {
        //    var progress = parseInt(data.loaded / data.total * 100, 10);
        //    data.context.find('.bar').progressbar({
        //        value: progress
        //    });
        //},
        fail: function (e, data, errorThrown) {
            $('#reset').trigger("click");
            var returnedMessage = "An Error occured. Please resubmit the file. If the problem persists please validate the content or contact user suport";
            if (typeof data.jqXHR.responseJSON.Message != "undefined") returnedMessage = data.jqXHR.responseJSON.Message;
            clearInterval(intervalId);
            resetButtons();
            alert(returnedMessage);

        }
    })
    //}).bind('fileuploadalways', function (e, data) {
    //    var currentFile = data.files[data.index];
    //    if (data.files.error && currentFile.error) {
    //        // there was an error, do something about it
    //        console.log(currentFile.error);
    //    }
    //});
    //;
    //$('#fileupload').bind('fileuploadadded', function (e, data) {
    //    console.log(data.files.valid);
    

    function reset_form_element(e) {
        e.wrap('<form>').parent('form').trigger('reset');
        e.unwrap();
        $("table tbody.files").empty();
        $('#loading').addClass('hide');
    }
    $('#reset').on('click', function (e) {
        reset_form_element($('#fileupload'));
        $('.fileinput-button').removeClass('disabled');
        $('#filelistholder').text("");
        $('#startupload').unbind('click')
        $('#startupload').addClass("disabled");
        $('#reset').addClass("disabled");
        $('#loading').addClass('hide');
        e.preventDefault();
    });

    function checkFileType(filename) {
        isValid = false
        // get the file name, possibly with path (depends on browser)
        //var filename = $("#file_input").val();

        // Use a regular expression to trim everything before final dot
        var extension = filename.replace(/^.*\./, '').toLowerCase();;

        if ((extension == "csv") || (extension == "zip"))
            isValid = true
        return isValid;

    }
    function resetButtons()
    {

        $('.fileinput-button').removeClass('disabled');
        $('#filelistholder').text("");
        $('#startupload').unbind('click')
        $('#startupload').addClass("disabled");
        $('#reset').addClass("disabled");
        $('#loading').addClass('hide');


    }

    function setupInterval()
    {
        intervalId = setInterval(funcName, 1000);

        
    }
  

    function interval(func, wait, times) {
        var interv = function (w, t) {
            return function () {
                if (typeof t === "undefined" || t-- > 0) {
                    setTimeout(interv, w);
                    try {
                        func.call(null);
                    }
                    catch (e) {
                        t = 0;
                        throw e.toString();
                    }
                }
            };
        }(wait, times);

        setTimeout(interv, wait);
    };

    function trackJobProgress(job) {
        setProgressBarWidth(job.Progress);
        $("#completedDisplay").hide();
        $('#progressDisplay').show();

        var hubProxy = $.connection.progressHub;

        hubProxy.client.progressChanged = function (jobId, progress) {
            setProgressBarWidth(progress);
        };

        hubProxy.client.jobCompleted = function (jobId) {
            $('#progressDisplay').hide();
            $("#completedDisplay").show();
            $("#startButton").prop('disabled', false);
            $.connection.hub.stop();
            window.location.href = '/CSVUpload/' + viewName

        };

        $.connection.hub.start().done(function () {
            hubProxy.server.trackJob(job.JobId);
        });
    }

    function setProgressBarWidth(progress) {
        $("#progresssBarValue").css("width", progress + "%");
    }

//});
function funcName() {

    //
    var jqxhr = $.post("/Home/Progress", function (progress) {
        updateMonitor(status, progress);
    })

    // $.post('Home/Progress', function (progress) {
    //   //if (progress >= 1000) {
    //   //    updateMonitor(taskId, "Completed");
    //   //    clearInterval(intervalId);
    //   //} else {
    //    updateMonitor(d, progress);
    //   //}
    //});

}
function updateMonitor(status, progress) {
        $('#monitor').html(progress);
    }