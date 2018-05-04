$(document).ready(function () {

    //Click handler for 'Clear selected tables' button
    $(".btn").click(function (event) {
        event.preventDefault();
        var f = $('#clearTables').serialize();
        bootbox.confirm("Are you sure you want to clear these tables?", function (result) {
            if (result) {

                $.ajax({
                    url: "ClearTablesHandler",
                    type: "POST",
                    data: $('#clearTables').serialize(),
                    success: function (data, textStatus) {
                        bootbox.alert("You successfully cleared these tables")
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        bootbox.alert("Delete not successful please make sure the referenced tables are cleared as well. e.g. methods table and  labmethods table  are references by datavalues table. ")
                    }
                })
            }
        })
    })

    //Disable 'Clear selected tables' button
    $('button[type="submit"]').addClass('disabled');

    //Click handler for any table checkbox...
    $('input[type="checkbox"]').on('click', function (event) {
        //Any check boxes selected?
        var checkedCount = $('input:checked').length;
        var jqButton = $('button[type="submit"]');

        if (0 < checkedCount) {
            //console.log('Checked: ' + checked.toString());
            jqButton.removeClass('disabled');            
        }
        else {
            //console.log('NONE checked');
            jqButton.addClass('disabled');
        }
    });
});