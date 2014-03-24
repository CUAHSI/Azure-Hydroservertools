$(document).ready(function () {
    $(".btn").click(function (event) {
        event.preventDefault();
        var f = $('#clearTables').serialize()
        if (!confirm("Are you sure you want to clear these tables?")) {
            return false;
        }
        $.ajax({
            url: "ClearTablesHandler",
            type: "POST",
            data: $('#clearTables').serialize(),
            success: function (data, textStatus) {
                alert("You successfully cleared these tables")
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("Delete not successful please make sure the referenced tables are cleared as well.")
            }
        })
    });
});