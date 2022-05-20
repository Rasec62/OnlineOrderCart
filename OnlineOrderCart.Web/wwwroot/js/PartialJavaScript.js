    function AddData() {
        $.get("@Url.Action('MyCreatePartial', 'Distributors')",
            function (data) {
                $('.modal-body').html(data);
            });
    $("#myModal").modal("show");
}
    function GetbyID(ID) {
        $.get("@Url.Action('GetByID', 'Distributors')/" + ID,
            function (data) {
                $('.modal-body').html(data);
            });
    $("#myModal").modal("show");
}
    function EditData() {
        $.post("@Url.Action('MyEditPartial', 'Distributors')",
            function (data) {
                $('.modal-body').html(data);
            });
    $("#myModal").modal("show");
}
    function GetbyDelID(ID) {
        $.get("@Url.Action('GetByDeleteID', 'Distributors')/" + ID,
            function (data) {
                $('.modal-body').html(data);
            });
    $("#myModal").modal("show");
}
    function GetDetailPartialsID(ID) {
        $.get("@Url.Action('MyDetailPartials', 'Distributors')/" + ID,
            function (data) {
                $('.modal-body').html(data);
            });
    $("#myModal").modal("show");
}
