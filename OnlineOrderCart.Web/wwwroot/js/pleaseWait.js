/**
 * Displays overlay with "Please wait" text. Based on bootstrap modal. Contains animated progress bar.
 */
function showPleaseWait() {

    if (document.querySelector("#pleaseWaitDialog") == null) {
        var modalLoading = '<div class="modal" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false" role="dialog">\
            <div class="modal-dialog modal-dialog-centered">\
                <div class="modal-content">\
                    <div class="modal-header">\
                        <h4 class="modal-title">Please wait...</h4>\
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">\
                             <span aria-hidden="true" >\
                                &times;\
                             </span >\
                            </button >\
                    </div>\
                    <div class="modal-body">\
                        <div class="row">\
                             <div class="col s10 text-center">\
                                <img src="/Image/LoadCat.gif" alt= "" style="width:250px; height:100px; max-width: 100%; height: auto; border-radius:50%;" />\
                            </div >\
                        </div >\
                    </div>\
                </div>\
            </div>\
        </div>';
        $(document.body).append(modalLoading);
    }

    $("#pleaseWaitDialog").modal("show");
}

/**
 * Hides "Please wait" overlay. See function showPleaseWait().
 */
function hidePleaseWait() {
    $("#pleaseWaitDialog").modal("hide");
}