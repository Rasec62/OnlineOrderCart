function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        };
        reader.readAsDataURL(imageUploader.files[0]);
    }
}
$(document).ready(function () {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd
    }
    if (mm < 10) {
        mm = '0' + mm
    }

    today = yyyy + '-' + mm + '-' + dd;
    document.getElementById("BirthDate").setAttribute("max", today);
    $(function () {


    });
});

$(function () {
    $.datepicker.setDefaults({
        dateFormat: 'yy-mm-dd', firstDay: 0,
        showOn: 'both', buttonImage: 'img/calendar.gif'
    });
    $('#BirthDate').datepicker({
        minDate: 0, onSelect: function (date) {
            date = $(this).datepicker('getDate');
            var maxDate = new Date(date.getTime());
            maxDate.setDate(maxDate.getDate() + 1);
            $('#endsession').datepicker('option', { minDate: date, maxDate: maxDate }).
                datepicker('setDate', date);
        }
    });
    $('#endsession').datepicker({ minDate: 0 });
});

$(function () {
    $("#txtDate").datepicker({
        changeMonth: true,
        changeYear: true,
        showOn: 'button',
        buttonImageOnly: true,
        buttonImage: 'images/calendar.gif',
        dateFormat: 'dd/mm/yyyy',
        yearRange: '-10:+10'   //Current Year -10 to Current Year + 10.
        //yearRange: '+0:+10'    //Current Year to Current Year + 10.
        //yearRange: '1900:+0'   //Year 1900 to Current Year.
        //yearRange: '1985:2025' //Year 1985 to Year 2025.
        //yearRange: '-0:+0'     //Only Current Year.
        //yearRange: '2025' //Only Year 2025.
    });
});