$(document).ready(function () {
    var items = "<option value='0'>Select a product</option>";
    $('#SubCategoryID').html(items);
    $('#ProductId').html(items);
});
$(document).ready(function () {
    $('#SubCategoryID').change(function () {
        var url = '@Url.Content("~/")' + "Distributors/GetSubProduct";
        var ddlsource = "#SimTypeId";
        $.getJSON(url, { SimTypeId: $(ddlsource).val() }, function (data) {
            var items = '';
            $("#SubCategoryID").empty();
            $.each(data, function (i, subcategory) {
                items += "<option value='" + subcategory.value + "'>" + subcategory.text + "</option>";
            });
            $('#SubCategoryID').html(items);
        });
    });

    $('#SimTypeId').change(function () {
        var url = '@Url.Content("~/")' + "Distributors/GetSubProduct";
        var ddlsource = "#SimTypeId";
        $.getJSON(url, { SimTypeId: $(ddlsource).val() }, function (data) {
            var items = '';
            $('#ProductId').empty();
            $.each(data, function (i, product) {
                items += "<option value='" + product.value + "'>" + product.text + "</option>";
            });
            $('#ProductId').html(items);
        });
    });
});

        