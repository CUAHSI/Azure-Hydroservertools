﻿//$(document).ready(function () {





//$(document).ready(function () {

    var oTable;
    var UploadMonitorID
    var commitMonitor = { 'intervalId': null }

    var sPath = window.location.pathname;
    var viewName = (sPath.substring(sPath.lastIndexOf('/') + 1)).toLowerCase();

    //get upload stats for current upload
    GetUploadStats(viewName);



    $("#tabs").tabs({

        "beforeActivate": function (event, ui) {
           
        },

        "activate": function (event, ui) {

            var index = $("#tabs").tabs('option', 'active');

            if ($("#tabs").tabs('option', 'active') === 0) {
                oTable = $('#0').dataTable(getDatatableOptions(viewName, 0));

                //oTable.fnReloadAjax("SitesSearch");
            } else if ($("#tabs").tabs('option', 'active') === 1) {
                // $('#1').dataTable().fnDestroy();
                oTable = $('#1').dataTable(getDatatableOptions(viewName, 1));

                //oTable.fnReloadAjax("SitesSearch");
            } else if ($("#tabs").tabs('option', 'active') === 2) {
                oTable = $('#2').dataTable(getDatatableOptions(viewName, 2));
                //toggleCommitButtons(oTable, 2);
                //oTable.fnReloadAjax("SitesSearch");
            } else {
                oTable = $('#3').dataTable(getDatatableOptions(viewName, 3));

                //oTable.fnReloadAjax("SitesSearch");
            }
            // var index = $("#tabs").tabs('option', 'active');

            //oTable.fnReloadAjax();

        }
    });





function getDatatableOptions(name, index) {

    var path;
    if ((typeof index != "undefined"))
        path = "/" + index;
    else
        path = "";

    var optDataTables;
    var sImageUrl = '/images/'
    switch (name.toLowerCase()) {
        case "sites":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Sites" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "200%",
                "sScrollYInner" : 600,
                "bScrollCollapse": true,
                "bDestroy": true,
                //"fnInitComplete": function (oSettings, json) {
                //    initMessageColumn(index)
                //    toggleCommitButtons(oTable, index);
                //},
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },

                //"bJQueryUI": "true",
                //"fnRender": function (oObj) {
                //    return "<a href='#'>Edit</a>";
                //},

                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "SiteCode" },
                    { "aTargets": [2], "sName": "SiteName" },
                    { "aTargets": [3], "sName": "Latitude" },
                    { "aTargets": [4], "sName": "Longitude" },
                    { "aTargets": [5], "sName": "LatLongDatumSRSName" },
                    { "aTargets": [6], "sName": "Elevation_m" },
                    { "aTargets": [7], "sName": "VerticalDatum" },
                    { "aTargets": [8], "sName": "LocalX" },
                    { "aTargets": [9], "sName": "LocalY" },
                    { "aTargets": [10], "sName": "LocalProjectionSRSName" },
                    { "aTargets": [11], "sName": "PosAccuracy_m" },
                    { "aTargets": [12], "sName": "State" },
                    { "aTargets": [13], "sName": "County" },
                    { "aTargets": [14], "sName": "Comments" },
                    { "aTargets": [15], "sName": "SiteType" },
                    { "aTargets": [16], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "variables":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Variables" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "150%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "VariableCode" },
                    { "aTargets": [2], "sName": "VariableName" },
                    { "aTargets": [3], "sName": "Speciation" },
                    { "aTargets": [4], "sName": "VariableUnitsName" },
                    { "aTargets": [5], "sName": "SampleMedium" },
                    { "aTargets": [6], "sName": "ValueType" },
                    { "aTargets": [7], "sName": "IsRegular" },
                    { "aTargets": [8], "sName": "TimeSupport" },
                    { "aTargets": [9], "sName": "TimeUnitsName" },
                    { "aTargets": [10], "sName": "DataType" },
                    { "aTargets": [11], "sName": "GeneralCategory" },
                    { "aTargets": [12], "sName": "NoDataValue" },
                    { "aTargets": [13], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "offsettypes":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "OffsetTypes" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    //{ "aTargets": [1], "sName": "OffsetTypeID", "bVisible": false },
                    { "aTargets": [1], "sName": "OffsetTypeCode" },
                    { "aTargets": [2], "sName": "OffsetUnitsName" },
                    { "aTargets": [3], "sName": "OffsetDescription" },                    
                    { "aTargets": [4], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "sources":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Sources" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "300%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"fnRowCallback": function (nRow, aData, iDisplayIndex) {
                //    $('td:eq(17)', nRow).html('<b>A</b>');
                //},
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "SourceCode", "sWidth": "110px" },
                    { "aTargets": [2], "sName": "Organization", "sWidth": "200px" },
                    { "aTargets": [3], "sName": "SourceDescription", "sWidth": "300px" },
                    {
                        "aTargets": [4], "sName": "SourceLink", "sWidth": "250px", "fnRender": function (aData, val) {
                            return '<a href="/' + val + '" target=_Blank>' + val + '</a>';
                        }
                    },
                    { "aTargets": [5], "sName": "ContactName" },
                    { "aTargets": [6], "sName": "Phone" },
                    { "aTargets": [7], "sName": "Email" },
                    { "aTargets": [8], "sName": "Address", "sWidth": "200px" },
                    { "aTargets": [9], "sName": "City" },
                    { "aTargets": [10], "sName": "State" },
                    { "aTargets": [11], "sName": "ZipCode" },
                    { "aTargets": [12], "sName": "Citation", "sWidth": "200px" },
                    { "aTargets": [13], "sName": "TopicCategory" },
                    { "aTargets": [14], "sName": "Title", "sWidth": "150px" },
                    { "aTargets": [15], "sName": "Abstract", "sWidth": "300px" },
                    { "aTargets": [16], "sName": "ProfileVersion" },
                    {
                        "aTargets": [17], "sName": "MetadataLink", "sWidth": "200px", "fnRender": function (aData, val) {
                            return '<a href="/' + val + '" target=_Blank>' + val + '</a>';
                        }

                    },
                    { "aTargets": [18], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "methods":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Methods" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "MethodCode" },
                    { "aTargets": [2], "sName": "MethodDescription" },
                    {
                        "aTargets": [3], "sName": "MethodLink", "fnRender": function (aData, val) {
                            return '<a href="/' + val + '" target=_Blank>' + val + '</a>';
                        }
                    },
                    { "aTargets": [4], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "labmethods":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "LabMethods" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "LabName" },
                    { "aTargets": [2], "sName": "LabOrganization" },
                    { "aTargets": [3], "sName": "LabMethodName" },
                    { "aTargets": [4], "sName": "LabMethodDescription" },
                    {
                        "aTargets": [5], "sName": "LabMethodLink", "fnRender": function (aData, val) {
                            return '<a href="/' + val + '" target=_Blank>' + val + '</a>';
                        }
                    },
                    { "aTargets": [6], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "samples":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Samples" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                   
                    { "aTargets": [1], "sName": "SampleType" },
                    { "aTargets": [2], "sName": "LabSampleCode" },
                    { "aTargets": [3], "sName": "LabMethodName" },                    
                    { "aTargets": [4], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "qualifiers":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Qualifiers" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },                    
                    { "aTargets": [1], "sName": "QualifierCode" },
                    { "aTargets": [2], "sName": "QualifierDescription" },
                    { "aTargets": [3], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "qualitycontrollevels":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "QualityControlLevels" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "QualityControlLevelCode", "sWidth": "200px" },
                    { "aTargets": [2], "sName": "Definition" },
                    { "aTargets": [3], "sName": "Explanation" },
                    { "aTargets": [4], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "datavalues":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Datavalues" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "250%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    //{ "aTargets": [1], "sName": "ValueID", "bVisible": false },
                    { "aTargets": [1], "sName": "DataValue" },
                    { "aTargets": [2], "sName": "ValueAccuracy" },
                    { "aTargets": [3], "sName": "LocalDateTime" },
                    { "aTargets": [4], "sName": "UTCOffset" },
                    { "aTargets": [5], "sName": "DateTimeUTC" },
                    { "aTargets": [6], "sName": "SiteCode" },
                    //{ "aTargets": [7], "sName": "VariableID", "bVisible": false },
                    { "aTargets": [7], "sName": "VariableCode" },
                    { "aTargets": [8], "sName": "OffsetValue" },
                    { "aTargets": [9], "sName": "OffsetTypeID" },
                    { "aTargets": [10], "sName": "CensorCode" },
                    { "aTargets": [11], "sName": "QualifierID" },
                    { "aTargets": [12], "sName": "MethodID" },
                    { "aTargets": [13], "sName": "MethodDescription", "bVisible": false },
                    { "aTargets": [14], "sName": "SourceID" },
                    { "aTargets": [15], "sName": "SampleID" },
                    { "aTargets": [16], "sName": "DerivedFromID" },
                    { "aTargets": [17], "sName": "QualityControlLevelCode" },
                    { "aTargets": [18], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            };
            break;
        case "groupdescriptions":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "GroupDescriptions" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "GroupID", "bVisible": false },
                    { "aTargets": [2], "sName": "GroupDescription" },
                    { "aTargets": [3], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "groups":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Groups" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                     {
                         "aTargets": [0], "sWidth": "50px",
                         "bSearchable": false,
                         "bSortable": false,
                         "fnRender": function (oObj) {
                             return addImageToColumn(index);
                         }
                     },
                     { "aTargets": [1], "sName": "GroupID" },
                     { "aTargets": [2], "sName": "ValueID", "sWidth": "300px" },
                     { "aTargets": [3], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "derivedfrom":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "DerivedFrom" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "DerivedFrom" },
                    { "aTargets": [2], "sName": "ValueID" },
                    { "aTargets": [3], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;
        case "categories":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Categories" + path,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "fnDrawCallback": function (oSettings) {
                    initMessageColumn(index)
                    toggleCommitButtons(oTable, index);
                },
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    {
                        "aTargets": [0], "sWidth": "50px",
                        "bSearchable": false,
                        "bSortable": false,
                        "fnRender": function (oObj) {
                            return addImageToColumn(index);
                        }
                    },
                    { "aTargets": [1], "sName": "VariableCode" },
                    { "aTargets": [2], "sName": "Datavalue" },
                    { "aTargets": [3], "sName": "CategoryDescription" },
                    { "aTargets": [4], "sName": "Errors", "bSortable": false, "bVisible": false }
                 ]
            }
            break;

    }
    return optDataTables;
}

function fnFormatDetails(rawMessage) {
    var s = "";
    if ((typeof (rawMessage) != 'undefined') && (rawMessage != null)) {
        var formattedMessage = rawMessage.split(";")

        for (var i = 0; i < formattedMessage.length - 1; i++) {
            s = s + "<tr><td>" + formattedMessage[i] + "</td></tr>";
        }

        sOut = '<div class="innerDetails" >' +
          '<table>' +
                      s
        '</table>' +
      '</div>';
    }
    else {
        sOut = "No additional Information available."
    }

    return sOut;
}

function initMessageColumn(index) {
    if ((index != 1) && (index != 2)) {
        $("tbody td img.expand").addClass("hide")
    }
    else {

        $("tbody td img.expand").removeClass("hide");
        $("tbody td img.expand").on("click", function () {
            var nTr = this.parentNode.parentNode;
            oTable = $('#' + index).dataTable();
            if (this.src.match('close')) {
                this.src = "/Images/open.png";
                oTable.fnClose(nTr);
            } else {
                this.src = "/Images/close.png";
                var orderId = $(this).attr("rel");
                //var url = "/Home/ResultDetailView";
                oTable = $('#' + index).dataTable();
                var aData = oTable.fnGetData(nTr);
                var errorColumnId = aData.length - 1;
                oTable.fnOpen(nTr, fnFormatDetails(aData[errorColumnId]), 'table-error-details');
                // $.get(url, { id: 1 }, function (details) {


                //});
            }
        });
    }
}

function addImageToColumn(index) {
    sOut = ""
    if ((index == 1) || (index == 2)) {
        sOut = "<img class='expand' src='/Images/open.png' alt='Expand/Collapse' rel='' />";
    }
    return sOut;
}

function initCommitAndCancelButton(id) {

    $('#0commit').click(function () {
        $('#0commit').addClass('disabled');
        $('#cancel').addClass('disabled');

        $('#loading').removeClass('hide');
        $('#0commit').unbind('click')
        $('#cancel').unbind('click')

        //UploadMonitorID = setInterval(function () {

        //    $.post("/Home/Progress", function (progress) {
        //        //if (progress >= 1000) {
        //        //    updateMonitor(taskId, "Completed");
        //        //    clearInterval(intervalId);
        //        //} else {
        //        updateMonitor(status, progress);
        //        //}
        //    });

        //}, 1000);

        //$.post("/CSVUpload/Commit", { id: "sites" } {

        startCommitMonitor();
        $.ajax({
            url: '/CSVUpload/Commit/' + id,
            data: "{ 'index': '0' }",
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function () {

                bootbox.alert("Records successfully added.");
                GetUploadStats(id);
                oTable = $('#0').dataTable(getDatatableOptions(id, 0));
               
                $('#0commit').addClass("disabled");
                //$('#loading').addClass('hide');
                $('#cancel').bind('click');
                // window.clearInterval(UploadMonitorID);
                clearInterval(commitMonitor.intervalId); 
                $('#monitor').html('');

            },
            always: function (e, data)
            {
                clearInterval(uploadMonitor.intervalId);
            },
            error: function (xhr) {
                if (typeof xhr.statusText != "undefined") {
                    returnedMessage = xhr.statusText;
                }
                else {
                    var returnedMessage = "An Error occured. Please resubmit the file. If the problem persists please validate the content or contact user suport";
                }

                $('#loading').addClass('hide');

                bootbox.alert(returnedMessage, function (reslt) {
                    
                        window.location.href = '/CSVUpload/UploadData/' + id
                    
                //window.clearInterval(UploadMonitorID);
                })
            },timeout:600000
        });

    });

    $('#2commit').click(function () {
        //$.post("/CSVUpload/Commit", { id: "sites" } {
        $('#2commit').addClass('disabled');
        $('#cancel').addClass('disabled');

        $('#loading').removeClass('hide');
        $('#2commit').unbind('click')
        $('#cancel').unbind('click')

        //intervalId = setInterval(function () {

        //    $.post("/Home/Progress", function (progress) {
        //        //if (progress >= 1000) {
        //        //    updateMonitor(taskId, "Completed");
        //        //    clearInterval(intervalId);
        //        //} else {
        //        updateMonitor(status, progress);
        //        //}
        //    });

        //}, 1000);

        $.ajax({
            url: '/CSVUpload/Commit/' + id,
            data: "{ 'index': '2' }",
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function () {
                bootbox.alert("Records successfully added.")
                oTable = $('#2').dataTable(getDatatableOptions(id, 2));
                GetUploadStats(id);
                $('#2commit').addClass("disabled");               
                $('#loading').addClass('hide');
                $('#cancel').bind('click');
            },
            error: function (xhr) {
                if (typeof xhr.statusText != "undefined") {
                    returnedMessage = xhr.statusText;
                }
                else {
                    var returnedMessage = "An Error occured. Please resubmit the file. If the problem persists please validate the content or contact user suport";
                }

                bootbox.alert(returnedMessage);
                window.location.href = '/CSVUpload/UploadData/' + id
            }
        });

    });
    $('.cancel').click(function () {
        $.ajax({
            url: '/CSVUpload/Cancel/' + id,
            data: "{ 'index': '0' }",
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function () {
                window.location.href = '/CSVUpload/UploadData/' + id
            },
            error: function () {
                bootbox.alert("error");
            }
        });
    });
    //$('#2cancel').click(function () {
    //    $.ajax({
    //        url: '/CSVUpload/Cancel/' + id,
    //        data: "{ 'index': '2' }",
    //        dataType: 'json',
    //        type: 'POST',
    //        contentType: 'application/json; charset=utf-8',
    //        success: function () {
    //            window.location.href = '/CSVUpload/' + id
    //        },
    //        error: function () {
    //            alert("error");
    //        }
    //    });
    //});
    // var r = $('<input type="button" value="new button"/>');

    //$('#0_filter').append(r)
    //$('#download').click(function () {
    //    //$.post("/CSVUpload/Commit", { id: "sites" } {
    //    $.ajax({
    //        url: '/Download/Export/' + id,
    //        data: {identifier:"0",viewName: "sites"},
    //        type: 'POST',
    //        contentType: 'application/json; charset=utf-8',
    //        success: function () {
    //            alert();
    //        },
    //        error: function () {
    //            alert("error");
    //        }
    //    });

    //});
}

function toggleCommitButtons(table, id) {
    if (table.fnGetData().length > 0) {
        $('#' + id + 'commit').removeClass("hide")
        //$('#' + id + 'cancel').removeClass("hide")
    }
    else {
        if (!$('#' + id + 'commit').hasClass("hide")) {
            $('#' + id + 'commit').addClass("hide")
        }
        //if (!$('#'+ id + 'cancel').hasClass("hide")) {
        // $('#'+ id + 'cancel').addClass("hide")
        //}
    }
}

function GetUploadStats(viewName)
{
    $.ajax({
        url: '/CSVUpload/GetUploadStatistics',
        data: { viewName: viewName },
        dataType: 'json',
        type: 'Post',
        success: function (data) {
            $('#NewBadge').html(data.NewRecordCount)
            $('#RejectedBadge').html(data.RejectedRecordCount)
            $('#UpdatedBadge').html(data.UpdatedRecordCount)
            $('#DuplicateBadge').html(data.DuplicateRecordCount)
            
              //
              // 
             //          
        },
        error: function (xhr) {
            if (typeof xhr.statusText != "undefined") {
                returnedMessage = xhr.statusText;
            }
            else {
                var returnedMessage = "An Error occured. Please resubmit the file. If the problem persists please validate the content or contact user suport";
            }

            bootbox.alert(returnedMessage);


        }
    });
}

function startCommitMonitor() {
    if (null === commitMonitor.intervalId) {
        //Monitor function not running - start...
        commitMonitor.intervalId = setInterval(function () {
            //var actionUrl = "/Home/Progress";
            var actionUrl = "/Home/Progress";
            $.ajax({
                url: actionUrl,
                type: 'POST',
                contentType: 'json',
                //data: JSON.stringify(Ids),
                success: function (progress) {
                    commitMonitor2(status, progress);

                },
                error: function (xmlhttprequest, textStatus, message) {
                    alert('error')
                }
            })
        }, 2000)
    }
}

function commitMonitor2(status, progress) {
    $('#monitor').html(progress);
    if (progress == "Processing Complete") {
         clearInterval(commitMonitor.intervalId);
    }
    if (progress.indexOf("Processing Failed", 0) > -1) {
        clearInterval(commitMonitor.intervalId);
    }
}
//function updateMonitor(status, progress) {
//    $('#monitor').html(progress);
//}

//});


