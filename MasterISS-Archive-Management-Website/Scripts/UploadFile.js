function SetupFileUploadAreas() {
    $('.drop-zone').each(function () {
        let currentFiles = [];
        var currentDropZone = $(this);
        var currentForm = currentDropZone.closest('form');

        currentDropZone.on('dragover', function (e) {
            e.stopPropagation();
            e.preventDefault();
        });

        currentDropZone.on("drop", function (e) {
            e.preventDefault();

            var files = e.originalEvent.dataTransfer.files;
            var columns = "";
            for (var i = 0; i < files.length; i++) {
                currentFiles.push(files[i]);
                columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em; margin-top:1em;' src='/Content/img/remove-file.svg' alt='image'>" + "<img style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image' >" + files[i].name + "</div>";
            }
            $(this).append("<div class='row'>" + columns + "</div>");
        });

        //currentForm.on('formdata', (e) => {
        //    let data = e.formData;
        //    for (var value of data.values()) {
        //        console.log(value);
        //    }
        //});

        currentForm.submit(function (e) {
            var ass = new FormData(this);
            //$(this).trigger('formdata');
            e.preventDefault();
        });
    });
    //var currentFiles = [];
    ////let fileInput = document.querySelector('.customerDropFile');
    ////var dropzone =$(".dropzone");
    //$(".dropzone").on('dragover', function (e) {
    //    e.stopPropagation();
    //    e.preventDefault();
    //});
    //$(".dropzone").on("drop", function (e) {
    //    //e.stopPropagation();
    //    e.preventDefault();

    //    //var files = e.target.files || (e.dataTransfer && e.dataTransfer.files);
    //    var files = e.originalEvent.dataTransfer.files;
    //    //$(this).find(".customerDropFile")[0].files = files;
    //    //$('.customerDropFile')[0].files = files;
    //    var columns = "";
    //    for (var i = 0; i < files.length; i++) {
    //        currentFiles.push(files[i]);
    //        columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em; margin-top:1em;' src='/Content/img/remove-file.svg' alt='image'>" + "<img style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image' >" + files[i].name + "</div>";
    //    }
    //    $(this).append("<div class='row'>" + columns + "</div>");
    //});
    //function RemoveFile(fileName) {
    //    var index = currentFiles.indexOf(fileName);
    //    var removed = currentFiles.splice(index, 1);
    //    $('div[data-file="' + fileName + '"]').remove();
    //}
    //$('.file-dialog').click(function (e) {
    //    var files = $('.customerContract')[0].files;
    //    for (var i = 0; i < files.length; i++) {
    //        RemoveFile(files[i].name);
    //    }
    //    $('.customerContract').trigger("click");
    //});
    //$('.customerContract').change(function (e) {
    //    var files = $(this)[0].files;
    //    var columns = "";
    //    for (var i = 0; i < files.length; i++) {
    //        currentFiles.push(files[i]);
    //        columns += "<div data-file='" + files[i].name + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveFile('" + files[i].name + "');" + " style='width:2em; margin-top:1em;' src='/Content/img/remove-file.svg' alt='image'>" + "<img style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image' >" + files[i].name + "</div>";
    //    }
    //    //columns += "<div data-file='"  + "' class='col-md-6 col-sm-6 col-xl-3'><img onclick=" + "RemoveAllFile();" + " style='width:5em;' src='/Content/img/remove-file.svg' alt='image'>" "</div>";

    //    $('.dropzone').append("<div class='row'>" + columns + "</div>");
    //});

    ////$(".upload-submit").submit(function () {
    ////$(this).closest('form').find('input[type=file]')[0].files = currentFiles;
    //////for (var i = 0; i < currentFiles.length; i++) {
    //////    //$(".customerDropFile")[i].files = currentFiles[i].files;

    //////    //$("#Files").push(currentFiles[i]);
    //////    //$("#Files")[i].files = currentFiles[i].files;        
    //////    //$("#Files").push(currentFiles[i]);
    //////    //$("#Files")[i] = currentFiles[i];
    //////}
    ////var temp = 'a';

    ////const formElem = document.querySelector('form');
    ////const formElem = $('#form')[0];



    ////const formElem = $('.upload-submit')[0];


    ////formElem.on('submit', (e) => {
    ////formElem.submit(function (e) {
    ////$('.upload-submit').submit(function (e) {
    ////    const formElem = $('.upload-submit');
    ////    e.preventDefault();
    ////    new FormData(formElem);
    ////});

    ////formElem.on('formdata', (e) => {
    ////    console.log('formdata fired');

    ////    let data = e.formData;
    ////    for (var value of data.values()) {
    ////        console.log(value);
    ////    }

    ////    let request = new XMLHttpRequest();
    ////request.open("POST", "/formHandler");
    ////request.send(data);

    ////$(".upload-submit").submit(function () {
    ////    const uploadFile = (files) => {
    ////        console.log("Uploading file...");
    ////        const API_ENDPOINT = "https://file.io";
    ////        const request = new XMLHttpRequest();
    ////        const formData = new FormData();

    ////        request.open("POST", API_ENDPOINT, true);
    ////        request.onreadystatechange = () => {
    ////            if (request.readyState === 4 && request.status === 200) {
    ////                console.log(request.responseText);
    ////            }
    ////        };

    ////        for (let i = 0; i < files.length; i++) {
    ////            formData.append(files[i].name, files[i])
    ////        }
    ////        request.send(formData);
    ////    };
    ////});

    ////const formElem = document.querySelector('.upload-submit');

    ////const formElem = $('.upload-submit');
    //const formElem = $(this);


    //formElem.on('submit', (e) => {
    //    // on form submission, prevent default
    //    e.preventDefault();

    //    // construct a FormData object, which fires the formdata event
    //    //new FormData(formElem);
    //    //var formData = new FormData(/*$('.upload-submit')*/)
    //    var formData = new FormData();
    //    for (let i = 0; i < currentFiles.length; i++) {
    //        formData.append("Files", currentFiles[i]);
    //    }
    //    //formData.append('Files', currentFiles[0].files);
    //});


    //formElem.on('formData', (e) => {
    //    console.log('formdata fired');

    //    let data = e.formData;
    //    for (var value of data.values()) {
    //        console.log(value);
    //    }

    //    let request = new XMLHttpRequest();
    //    request.open("POST", "/formHandler");
    //    request.send(data);
    //});



    //$("#Files").on("change", event => {
    //    const files = event.target.files;
    //    uploadFile(files);
    //})
    //});



    //});

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
    //}
}
