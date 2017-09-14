$(document).ready(function () {
    $(".btn").click(function (event) {
        event.preventDefault();
        var f = $('#clearTables').serialize();
        bootbox.confirm("Are you sure you want to clear these tables?", function (result) {
            if (result) 
            {
                
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
           
            else  return false;   
        })
    })
});