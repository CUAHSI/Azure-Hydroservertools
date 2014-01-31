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
 
    $("#tabs").tabs({
        "show": function (event, ui) {
            var oTable = $('div.dataTables_scrollBody>table.display', ui.panel).dataTable();
            if (oTable.length > 0) {
                oTable.fnAdjustColumnSizing();
            }
        }
    });


    $("#tabs").tabs({
        "activate": function (event, ui) {
            var table = $.fn.dataTable.fnTables(true);
            if (table.length > 0) {
                $(table).dataTable().fnAdjustColumnSizing();
            }

          

        }

    });
});