$(document).ready(function () {

    var oTable = $('table.display').dataTable({
        "sScrollX": "100%",
        //"sScrollY": 400,
        "sScrollXInner": "150%",
        "bScrollCollapse": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "bJQueryUI": true,
        "sDom": '<"clear">Tlfrtip',
        //"sCharSet": "UTF16LE",
        "oTableTools": {
            "sSwfPath": "/Content/DataTables-1.9.4/extras/TableTools/media/swf/copy_csv_xls_pdf.swf"
        },
        "aoColumnDefs": [
            { "sWidth": "10%", "aTargets": [-1] }
        ]
    });
 
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


    $("#tabs").tabs({
        activate: function (event, ui) {
            var oTable = $('div.dataTables_scrollBody>table.display', ui.panel).dataTable();
            if (oTable.length > 0) {
                var oTableTools = TableTools.fnGetInstance(oTable[0]);
                oTable.dataTable().fnAdjustColumnSizing();
                oTableTools.fnResizeButtons();
               // alert("buttons should work");
            }

          

        }

    });
    $('[data-toggle="popover"]').popover({
        trigger: 'hover',
        'placement': 'top'
    });


});