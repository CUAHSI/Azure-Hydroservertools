//$(document).ready(function () {

//var oTable = $('table.display').dataTable({
//    "sScrollX": "100%",
//    //"sScrollY": 400,
//    "sScrollXInner": "150%",
//    "bScrollCollapse": true,
//    "sPaginationType": "full_numbers",
//    "bPaginate": true,
//    "bJQueryUI": true,
//    "sDom": '<"clear">Tlfrtip',
//    //"sCharSet": "UTF16LE",
//    "oTableTools": {
//        "sSwfPath": "/Content/DataTables-1.9.4/extras/TableTools/media/swf/copy_csv_xls_pdf.swf"
//    },
//    "aoColumnDefs": [
//        { "sWidth": "10%", "aTargets": [-1] }
//    ]
//});

//$("#tabs", tabs-1).tabs({
//    "show": function (event, ui) {
//        var oTable = $('div.dataTables_scrollBody>table.display', ui.panel).dataTable();
//        if (oTable.length > 0) {
//            oTable.fnAdjustColumnSizing();
//            oTable.fnResizeButtons();
//            alert("buttons should work");
//        }
//    }
//});



//$('[data-toggle="popover"]').popover({
//    trigger: 'hover',
//    'placement': 'top'
//});


//});

var oTable;

$(document).ready(function () {

    var sPath = window.location.pathname;
    var viewName = sPath.substring(sPath.lastIndexOf('/') + 1);

    $("#tabs").tabs({

        "beforeActivate": function (event, ui) {
            //oTable = $('#0').dataTable(getDatatableOptions("sites",0));
            //$('#1').dataTable().fnDestroy();
            //$('#2').dataTable().fnDestroy();
            //$('#3').dataTable().fnDestroy();

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
                "sScrollXInner": "150%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"fnInitComplete": function (oSettings, json) {
                //    initMessageColumn(index)
                //    toggleCommitButtons(oTable, index);
                //},
                "fnDrawCallback": function( oSettings ) {
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
                    { "aTargets": [10], "sName": "LocalProjectionID", "bVisible": false },
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
                    { "aTargets": [1], "sName": "OffsetTypeID", "bVisible": false },
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
                    { "aTargets": [1], "sName": "SourceID", "sWidth": "110px", "bVisible": false },
                    { "aTargets": [2], "sName": "Organization", "sWidth": "200px" },
                    { "aTargets": [3], "sName": "SourceDescription", "sWidth": "300px" },
                    { "aTargets": [4], "sName": "SourceLink", "sWidth": "250px" },
                    { "aTargets": [5], "sName": "ContactName" },
                    { "aTargets": [6], "sName": "Phone" },
                    { "aTargets": [7], "sName": "Email" },
                    { "aTargets": [8], "sName": "Address", "sWidth": "200px" },
                    { "aTargets": [9], "sName": "City" },
                    { "aTargets": [10], "sName": "State" },
                    { "aTargets": [11], "sName": "ZipCode" },
                    { "aTargets": [12], "sName": "Citation", "sWidth": "200px" },
                    { "aTargets": [13], "sName": "TopicCategory" },
                    { "aTargets": [14], "sName": "Title", "sWidth": "100px" },
                    { "aTargets": [15], "sName": "Abstract", "sWidth": "300px" },
                    { "aTargets": [16], "sName": "ProfileVersion" },
                    { "aTargets": [17], "sName": "MetadataLink", "sWidth": "200px" },
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
                    { "aTargets": [1], "sName": "MethodId", "bVisible": false },
                    { "aTargets": [2], "sName": "MethodDescription" },
                    { "aTargets": [3], "sName": "MethodLink" },
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
                    { "aTargets": [1], "sName": "LabMethodID", "bVisible": false },
                    { "aTargets": [2], "sName": "LabName" },
                    { "aTargets": [3], "sName": "LabOrganization" },
                    { "aTargets": [4], "sName": "LabMethodName" },
                    { "aTargets": [5], "sName": "LabMethodDescription" },
                    { "aTargets": [6], "sName": "LabMethodLink" },
                    { "aTargets": [7], "sName": "Errors", "bSortable": false, "bVisible": false }
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
                    { "aTargets": [1], "sName": "SampleID", "bVisible": false },
                    { "aTargets": [2], "sName": "SampleType" },
                    { "aTargets": [3], "sName": "LabSampleCode" },
                    { "aTargets": [4], "sName": "LabMethodName", "bVisible": false },
                    { "aTargets": [5], "sName": "LabMethodID" },
                    { "aTargets": [6], "sName": "Errors", "bSortable": false, "bVisible": false }
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
                    { "aTargets": [1], "sName": "QualifierID", "bVisible": false },
                    { "aTargets": [2], "sName": "QualifierCode" },
                    { "aTargets": [3], "sName": "QualifierDescription" },
                    { "aTargets": [4], "sName": "Errors", "bSortable": false, "bVisible": false }
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
                    { "aTargets": [1], "sName": "QualityControlLevelCode", "sWidth": "110px" },
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
                    { "aTargets": [1], "sName": "ValueID", "bVisible": false },
                    { "aTargets": [2], "sName": "DataValue" },
                    { "aTargets": [3], "sName": "ValueAccuracy" },
                    { "aTargets": [4], "sName": "LocalDateTime" },
                    { "aTargets": [5], "sName": "UTCOffset" },
                    { "aTargets": [6], "sName": "DateTimeUTC" },
                    { "aTargets": [7], "sName": "SiteCode" },
                    //{ "aTargets": [7], "sName": "VariableID", "bVisible": false },
                    { "aTargets": [8], "sName": "VariableCode" },
                    { "aTargets": [9], "sName": "OffsetValue" },
                    { "aTargets": [10], "sName": "OffsetTypeID" },
                    { "aTargets": [11], "sName": "CensorCode" },
                    { "aTargets": [12], "sName": "QualifierID" },
                    { "aTargets": [13], "sName": "MethodID" },
                    { "aTargets": [14], "sName": "MethodDescription", "bVisible": false },
                    { "aTargets": [15], "sName": "SourceID" },
                    { "aTargets": [16], "sName": "SampleID" },
                    { "aTargets": [17], "sName": "DerivedFromID" },
                    { "aTargets": [18], "sName": "QualityControlLevelCode" },
                    { "aTargets": [19], "sName": "Errors", "bSortable": false, "bVisible": false }
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

function initMessageColumn(index)
{
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
                var errorColumnId = aData.length-1;
                oTable.fnOpen(nTr, fnFormatDetails(aData[errorColumnId]), 'table-error-details');
                // $.get(url, { id: 1 }, function (details) {


                //});
            }
        });
    }
}

function addImageToColumn(index)
{
    sOut = ""
    if ((index == 1) ||  (index == 2))  {
        sOut = "<img class='expand' src='/Images/open.png' alt='Expand/Collapse' rel='' />";
    }
    return sOut;
}

function initCommitAndCancelButton(id) {

    $('#0commit').click(function () {
        //$.post("/CSVUpload/Commit", { id: "sites" } {
        $.ajax({
            url: '/CSVUpload/Commit/' + id,
            data: "{ 'index': '0' }",
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function () {
               
                alert("Records successfully added.")
                oTable = $('#0').dataTable(getDatatableOptions(id, 0));
                $('#0commit').addClass("disabled");
            },
            error: function () {
                var returnedMessage = "An Error occured. Please resubmit the file. If the problem persists please validate the content or contact user suport";
               // if (typeof data.jqXHR.responseJSON.Message != "undefined") returnedMessage = data.jqXHR.responseJSON.Message;

                alert(returnedMessage);
              
            }
        });

    });
    $('#2commit').click(function () {
        //$.post("/CSVUpload/Commit", { id: "sites" } {
        $.ajax({
            url: '/CSVUpload/Commit/' + id,
            data: "{ 'index': '2' }",
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function () {
                alert("Records successfully added.")
                oTable = $('#2').dataTable(getDatatableOptions(id, 0));
                $('#2commit').addClass("disabled");
            },
            error: function () {
                alert("error");
            }
        });

    });
    $('#cancel').click(function () {
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
                alert("error");
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

function toggleCommitButtons(table, id)
{
    if (table.fnGetData().length > 0 )
    {
        $('#' + id + 'commit').removeClass("hide")
        //$('#' + id + 'cancel').removeClass("hide")
    }
    else
    {
        if(!$('#'+id+'commit').hasClass("hide"))
        {
            $('#'+id+'commit').addClass("hide")
        }
        //if (!$('#'+ id + 'cancel').hasClass("hide")) {
            // $('#'+ id + 'cancel').addClass("hide")
        //}
    }
}
