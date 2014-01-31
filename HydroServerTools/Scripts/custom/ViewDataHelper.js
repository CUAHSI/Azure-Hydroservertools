$(document).ready(function () {
    $('#table_id').dataTable({
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/Content/DataTables-1.9.4/extras/TableTools/media/swf/copy_csv_xls_pdf.swf"
        },
        "sScrollX": "100%",
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "sScrollXInner": "150%",
        "bScrollCollapse": true,
        "bJQueryUI": true,
       
    
        //"bAutoWidth": false,
        //"aoColumns":
        //    [
        //        //{ "sWidth": "5%" }, // 1st column width
        //        { "sWidth": "10%" }, // 2nd column width
        //        { "sWidth": "15%" }, // 3rd column width and so on
        //        { "sWidth": "5%" }, // 4th column width
        //        { "sWidth": "5%" }, // 5th column width
        //        { "sWidth": "5%" }, // 6th column width and so on
        //        { "sWidth": "5%" }, // 7th column width
        //        { "sWidth": "5%" }, // 8th column width
        //        { "sWidth": "5%" }, // 9th column width and so on
        //        { "sWidth": "5%" }, // 10th column width
        //        { "sWidth": "5%" }, // 11th column width
        //        { "sWidth": "5%" }, // 12th column width and so on
        //        { "sWidth": "5%" }, // 13th column width
        //        { "sWidth": "5%" }, // 14th column width
        //        { "sWidth": "10%" }, // 15th column width and so on
        //        { "sWidth": "10%" } // 16th column width and so on
        //    ]
    });
    $("#table_id").show(function () {
            var table = $.fn.dataTable.fnTables(true);
            if (table.length > 0) {
                $(table).dataTable().fnAdjustColumnSizing();
            }
        }
    )

});


