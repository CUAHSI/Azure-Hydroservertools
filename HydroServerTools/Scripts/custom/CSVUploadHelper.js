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
            var oTable;
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
                //oTable.fnReloadAjax("SitesSearch");
            } else {
                oTable = $('#3').dataTable(getDatatableOptions(viewName, 3));
                //oTable.fnReloadAjax("SitesSearch");
            }
            // var index = $("#tabs").tabs('option', 'active');

            //oTable.fnReloadAjax();




            //oTable = $('div.dataTables_scrollBody>table.display', ui.panel).dataTable();
            if (oTable.length > 0) {
                //      var oTableTools = TableTools.fnGetInstance(oTable[0]);
                //      oTable.dataTable().fnAdjustColumnSizing();
                //      oTableTools.fnResizeButtons();
                //      // alert("buttons should work");
            }
        }
    });

    $.fn.dataTableExt.oApi.fnReloadAjax = function (oSettings, sNewSource, fnCallback, bStandingRedraw) {
        // DataTables 1.10 compatibility - if 1.10 then versionCheck exists.
        // 1.10s API has ajax reloading built in, so we use those abilities
        // directly.
        if ($.fn.dataTable.versionCheck) {
            var api = new $.fn.dataTable.Api(oSettings);

            if (sNewSource) {
                api.ajax.url(sNewSource).load(fnCallback, !bStandingRedraw);
            }
            else {
                api.ajax.reload(fnCallback, !bStandingRedraw);
            }
            return;
        }

        if (sNewSource !== undefined && sNewSource !== null) {
            oSettings.sAjaxSource = sNewSource;
        }

        // Server-side processing should just call fnDraw
        if (oSettings.oFeatures.bServerSide) {
            this.fnDraw();
            return;
        }

        this.oApi._fnProcessingDisplay(oSettings, true);
        var that = this;
        var iStart = oSettings._iDisplayStart;
        var aData = [];

        this.oApi._fnServerParams(oSettings, aData);

        oSettings.fnServerData.call(oSettings.oInstance, oSettings.sAjaxSource, aData, function (json) {
            /* Clear the old information from the table */
            that.oApi._fnClearTable(oSettings);

            /* Got the data - add it to the table */
            var aData = (oSettings.sAjaxDataProp !== "") ?
                that.oApi._fnGetObjectDataFn(oSettings.sAjaxDataProp)(json) : json;

            for (var i = 0 ; i < aData.length ; i++) {
                that.oApi._fnAddData(oSettings, aData[i]);
            }

            oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();

            that.fnDraw();

            if (bStandingRedraw === true) {
                oSettings._iDisplayStart = iStart;
                that.oApi._fnCalculateEnd(oSettings);
                that.fnDraw(false);
            }

            that.oApi._fnProcessingDisplay(oSettings, false);

            /* Callback user function - for event handlers etc */
            if (typeof fnCallback == 'function' && fnCallback !== null) {
                fnCallback(oSettings);
            }
        }, oSettings);
    };


});
function getDatatableOptions(name, index) {


    if ((typeof index != "undefined"))
        index = "/" + index;
    else
        index = "";

    var optDataTables;
    switch (name.toLowerCase()) {
        case "sites":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Sites" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "150%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "SiteCode" },
                    { "aTargets": [1], "sName": "SiteName" },
                    { "aTargets": [2], "sName": "Latitude" },
                    { "aTargets": [3], "sName": "Longitude" },
                    { "aTargets": [4], "sName": "LatLongDatumSRSName" },
                    { "aTargets": [5], "sName": "Elevation_m" },
                    { "aTargets": [6], "sName": "VerticalDatum" },
                    { "aTargets": [7], "sName": "LocalX" },
                    { "aTargets": [8], "sName": "LocalY" },
                    { "aTargets": [9], "sName": "LocalProjectionID", "bVisible": false },
                    { "aTargets": [10], "sName": "PosAccuracy_m" },
                    { "aTargets": [11], "sName": "State" },
                    { "aTargets": [12], "sName": "County" },
                    { "aTargets": [13], "sName": "Comments" },
                    { "aTargets": [14], "sName": "SiteType" }
                 ]
            }
            break;
        case "variables":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Variables" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "150%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    //{ "aTargets": [0], "sName": "VariableID", "bVisible": false },
                    { "aTargets": [0], "sName": "VariableCode" },
                    { "aTargets": [1], "sName": "VariableName" },
                    { "aTargets": [2], "sName": "Speciation" },
                    { "aTargets": [3], "sName": "VariableUnitsName" },
                    { "aTargets": [4], "sName": "SampleMedium" },
                    { "aTargets": [5], "sName": "ValueType" },
                    { "aTargets": [6], "sName": "IsRegular" },
                    { "aTargets": [7], "sName": "TimeSupport" },
                    { "aTargets": [8], "sName": "TimeUnitsName" },
                    { "aTargets": [9], "sName": "DataType" },
                    { "aTargets": [10], "sName": "GeneralCategory" },
                    { "aTargets": [11], "sName": "NoDataValue" }
                 ]
            }
            break;
        case "offsettypes":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "OffsetTypes" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "OffsetTypeID", "bVisible": false },
                    { "aTargets": [1], "sName": "OffsetUnitsName" },
                    { "aTargets": [2], "sName": "OffsetDescription" }
                 ]
            }
            break;
        case "sources":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Sources" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "300%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "SourceID", "sWidth": "110px", "bVisible": false  },
                    { "aTargets": [1], "sName": "Organization", "sWidth": "200px" },
                    { "aTargets": [2], "sName": "SourceDescription", "sWidth": "300px" },
                    { "aTargets": [3], "sName": "SourceLink", "sWidth": "250px" },
                    { "aTargets": [4], "sName": "ContactName" },
                    { "aTargets": [5], "sName": "Phone" },
                    { "aTargets": [6], "sName": "Email" },
                    { "aTargets": [7], "sName": "Address", "sWidth": "200px" },
                    { "aTargets": [8], "sName": "City" },
                    { "aTargets": [9], "sName": "State" },
                    { "aTargets": [10], "sName": "ZipCode" },
                    { "aTargets": [11], "sName": "Citation", "sWidth": "200px" },
                    { "aTargets": [12], "sName": "TopicCategory" },
                    { "aTargets": [13], "sName": "Title", "sWidth": "100px" },
                    { "aTargets": [14], "sName": "Abstract", "sWidth": "300px" },
                    { "aTargets": [15], "sName": "ProfileVersion" },
                    { "aTargets": [16], "sName": "MetadataLink", "sWidth": "200px" }
                 ]
            }
            break;
        case "methods":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Methods" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "MethodId", "bVisible": false },
                    { "aTargets": [1], "sName": "MethodDescription" },
                    { "aTargets": [2], "sName": "MethodLink" }
                 ]
            }
            break;
        case "labmethods":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "LabMethods" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "LabMethodID", "bVisible": false },
                    { "aTargets": [1], "sName": "LabName" },
                    { "aTargets": [2], "sName": "LabOrganization" },
                    { "aTargets": [3], "sName": "LabMethodName" },
                    { "aTargets": [4], "sName": "LabMethodDescription" },
                    { "aTargets": [5], "sName": "LabMethodLink" }
                 ]
            }
            break;
        case "samples":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Samples" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "SampleID", "bVisible": false },
                    { "aTargets": [1], "sName": "SampleType" },
                    { "aTargets": [2], "sName": "LabSampleCode" },
                    { "aTargets": [3], "sName": "LabMethodName" }
                 ]
            }
            break;
        case "qualifiers":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Qualifiers" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "QualifierID", "bVisible": false },
                    { "aTargets": [1], "sName": "QualifierCode" },
                    { "aTargets": [2], "sName": "QualifierDescription" }
                 ]
            }
            break;
        case "qualitycontrollevels":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "QualityControlLevels" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "QualityControlLevelCode", "sWidth": "110px" },
                    { "aTargets": [1], "sName": "Definition" },
                    { "aTargets": [2], "sName": "Explanation" }
                 ]
            }
            break;
        case "datavalues":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Datavalues" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "250%",
                "bScrollCollapse": true,
                "bDestroy": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "ValueID", "bVisible": false },
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
                    { "aTargets": [17], "sName": "QualityControlLevelCode" }
                 ]
            };
            break;
        case "groupdescriptions":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "GroupDescriptions" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "GroupID", "bVisible": false  },
                    { "aTargets": [1], "sName": "GroupDescription" }
                 ]
            }
            break;
        case "groups":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Groups" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "GroupID" },
                    { "aTargets": [1], "sName": "ValueID", "sWidth": "300px" }
                 ]
            }
            break;
        case "derivedfrom":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "DerivedFrom" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "DerivedFrom" },
                    { "aTargets": [1], "sName": "ValueID" }
                 ]
            }
            break;
        case "categories":
            optDataTables = {
                "bServerSide": true,
                "sAjaxSource": "Categories" + index,
                "bProcessing": true,
                "sServerMethod": "POST",
                "sPaginationType": "full_numbers",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,
                "bDestroy": true,
                //"bRetrieve": true,
                "aoColumnDefs":
                 [
                    { "aTargets": [0], "sName": "VariableCode" },
                    { "aTargets": [1], "sName": "Datavalue" },
                    { "aTargets": [2], "sName": "CategoryDescription" }
                 ]
            }
            break;
       
    }
    return optDataTables;
}



