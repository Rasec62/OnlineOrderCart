var dataTable;

$(document).ready(function () {
    $("#DistributorId").on("change", function () {
        dataTable = $('#example').DataTable({
            paging: false,
            "ajax": {
                "type": "POST",
                "url": "/OrderIncentive/SpecialistDetails",
                "data": { "distributorId": $("#DistributorId").val() },
                "datatype": "json"
            },
            "columns": [
                { "data": "Debtor", "width": "20%" },
                { "data": "BusinessName", "width": "40%" },
                { "data": "ShippingBranchNo", "width": "20%" },
                { "data": "ShippingBranchName", "width": "20%" },
                { "data": "OraclepId", "width": "20%" },
                { "data": "DeatilProducts", "width": "20%" },
                { "data": "Quantity", "width": "20%" },
                { "data": "PaymentMethod", "width": "20%" },
                { "data": "UseCfdi", "width": "20%" },
                { "data": "OrderCode", "width": "20%" },
                { "data": "ShortDescription", "width": "20%" },
                {
                    "data": "Debtor",
                    "render": function (data) {
                        //return `<div class="text-center">
                        //        <a href="/Categorias/Edit/${data}" class="btn btn-outline-success text-dark" style="cursor-pointer;">Editar</a>
                        //        &nbsp;
                        //         <a onclick=Delete("/Categorias/Delete/${data}") class="btn btn-outline-danger text-dark" style="cursor-pointer;">Borrar</a>
                        //        </div>`;
                    }, "width": "20%"
                }
            ]
        });
    });
});

