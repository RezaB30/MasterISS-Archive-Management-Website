function Set() {

    const tableContent = $('.tablediv');
    const tableButtons = $('.tablebuttons');

    const deleteBack = $('.backgrey');
    const deletepopup = $('.popup');
    const deleteAskCancelBut = $(
        '.popup ul li:first-of-type a'
    );
    const deleteAskDeleteBut = $(
        '.popup ul li:last-of-type a'
    );

    deleteBack.css("display", "none");
    deletepopup.css("display", "none");

    function popUpper() {
        deleteBack.css("display", "block");
        deletepopup.css("display", "block");
        deleteBack.removeClass("backgreyanimback");
        deleteBack.addClass('backgreyanim');
        deletepopup.removeClass("backgreyanimback");
        deletepopup.addClass('popupanim');
    }
    function popDowner() {
        deleteBack.removeClass('backgreyanim');
        deleteBack.classList.addClass('backgreyanimback');
        deletepopup.removeClass('popupanim');
        deletepopup.addClass('popupanimback');
        setTimeout(function () {
            deleteBack.css("display", "none");
            deletepopup.css("display", "none");
        }, 500);
    }

    deleteAskCancelBut.on('click', popDowner);
    deleteAskDeleteBut.on('click', popDowner);
    deleteBack.on('click', popDowner);

    function checkForDeleteBut(event) {
        event.preventDefault();
        if (event.target.classList.contains('deletebutton')) {

        //if (event.target.hasClass('deletebutton')) {
            popUpper();
            //event.preventDefault();
        }
    }
    document.addEventListener('click', checkForDeleteBut);

}