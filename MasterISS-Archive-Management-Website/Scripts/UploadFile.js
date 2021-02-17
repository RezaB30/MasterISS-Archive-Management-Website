var currentFiles = [];
let fileInput = document.querySelector('#customerDropFile');
var dropzone = document.getElementById("dropzone");
dropzone.addEventListener('dragover', function (e) {
    e.stopPropagation();
    e.preventDefault();
});
dropzone.addEventListener("drop", function (e) {
    e.stopPropagation();
    e.preventDefault();
    var files = e.dataTransfer.files;
    fileInput.files = files;
    var columns = "";
    for (var i = 0; i < files.length; i++) {
        currentFiles.push(files[i]);
        columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em;' src='/Content/img/remove-file.svg' alt='image'>" + files[i].name + "</div>";
    }
    $('#dropzone').append("<div class='row'>" + columns + "</div>");
});
function RemoveFile(fileName) {
    var index = currentFiles.indexOf(fileName);
    var removed = currentFiles.splice(index, 1);
    $('div[data-file="' + fileName + '"]').remove();
}
$('#file-dialog').click(function (e) {
    var files = $('#customerContract')[0].files;
    for (var i = 0; i < files.length; i++) {
        RemoveFile(files[i].name);
    }
    $('#customerContract').trigger("click");
});
$('#customerContract').change(function (e) {
    var files = $(this)[0].files;
    var columns = "";
    for (var i = 0; i < files.length; i++) {
        currentFiles.push(files[i]);
        columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em;' src='/Content/img/remove-file.svg' alt='image'>" + files[i].name + "</div>";
    }
    $('#dropzone').append("<div class='row'>" + columns + "</div>");
});
function UploadFile() {
    for (var i = 0; i < currentFiles.length; i++) {
        $('#contract-form').append("<input name='selectedFiles' class='d-none' value='" + currentFiles[i].name + "'>");
    }
    $('#contract-form').submit();
}