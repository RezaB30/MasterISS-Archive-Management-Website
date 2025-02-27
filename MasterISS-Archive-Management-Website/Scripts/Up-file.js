﻿function SetupFileUploadAreas() {
    $('form.upload-submit').each(function(e) {
        var currentForm = $(this);
        var dropZone = currentForm.find('.drop-zone');
        var fileInputContainer = currentForm.find('div.file-input-container');
        var fileListName = currentForm.find('input.file-input-name').val();
        //currentForm.submit((e) => {
        //    e.preventDefault();
        //});
        dropZone.on('dragover', function (e) {
            e.stopPropagation();
            e.preventDefault();
        });

        dropZone.on('drop', (e) => {
            e.preventDefault();
            var draggedFiles = e.originalEvent.dataTransfer.files;
            fileInputContainer.append('<input type="file" class="fileClass" name="' + fileListName + '"/>');
            var createdInput = fileInputContainer.find('input[type=file]').last();
            createdInput[0].files = draggedFiles;

            //dropZone.append = "<div alt='image'><img style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image' ></div>";
            //dropZone.append = "<div style = 'width:3em; margin-top:1em;' src = '/Content/img/file.svg' alt = 'image'></div>";
            for (var i = 0; i < draggedFiles.length; i++) {
                dropZone.append("<div data-file='" + draggedFiles[i].name + "' class='fileIcon' ><img  style = 'width: 3em; margin-top: 0em; margin-left: 1.5em;' src='/Content/img/file-icon.png'><label style ='margin-left:1em; font-weight:500'>" + draggedFiles[i].name + "</label></div>");

            }

        });
    });
}