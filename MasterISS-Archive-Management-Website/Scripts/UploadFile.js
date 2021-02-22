var currentFiles = [];
//let fileInput = document.querySelector('.customerDropFile');
//var dropzone =$(".dropzone");
$(".dropzone").on('dragover', function (e) {
    e.stopPropagation();
    e.preventDefault();
});
$(".dropzone").on("drop", function (e) {
    //e.stopPropagation();
    e.preventDefault();

    var files = e.originalEvent.dataTransfer.files;
    $(this).find(".customerDropFile")[0].files = files;
    //$('.customerDropFile')[0].files = files;
    var columns = "";
    for (var i = 0; i < files.length; i++) {
        currentFiles.push(files[i]);
        columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em; margin-top:1em;' src='/Content/img/remove-file.svg' alt='image'>" + "<img style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image' >" + files[i].name + "</div>";
    }
    $(this).append("<div class='row'>" + columns + "</div>");
});
function RemoveFile(fileName) {
    var index = currentFiles.indexOf(fileName);
    var removed = currentFiles.splice(index, 1);
    $('div[data-file="' + fileName + '"]').remove();
}
$('.file-dialog').click(function (e) {
    var files = $('.customerContract')[0].files;
    for (var i = 0; i < files.length; i++) {
        RemoveFile(files[i].name);
    }
    $('.customerContract').trigger("click");
});
$('.customerContract').change(function (e) {
    var files = $(this)[0].files;
    var columns = "";
    for (var i = 0; i < files.length; i++) {
        currentFiles.push(files[i]);
        columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em; margin-top:1em;' src='/Content/img/remove-file.svg' alt='image'>" + "<img style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image' >" + files[i].name + "</div>";
    }
    //columns += "<div data-file='"  + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveAllFile();" + " style='width:5em;' src='/Content/img/remove-file.svg' alt='image'>" "</div>";

    $('.dropzone').append("<div class='row'>" + columns + "</div>");
});

//function UploadFile() {
//    for (var i = 0; i < currentFiles.length; i++) {
//        $('#contract-form').append("<input name='selectedFiles' class='d-none' value='" + currentFiles[i].name + "'>");
//    }
//    $('#contract-form').submit();
//}

//function Clear(fileName) {
//    var index = currentFiles.indexOf(fileName);
//    var removed = currentFiles.splice(index, 1);
//    $('div[data-file="' + fileName + '"]').remove();
//}


//function RemoveAllFile(fileName) {
//    for (var i = 0; i < file.columns; i++) {
//        var index = currentFiles.indexOf(fileName);
//        var removed = currentFiles.splice(index, i + 1);
//        $('div[data-file="' + fileName + '"]').RemoveAllFile();
//    }

//}