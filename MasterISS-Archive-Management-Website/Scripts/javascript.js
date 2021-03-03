function Set() {
    //const tableContent = document.querySelector('.tablediv');
    //const tableButtons = document.querySelector('.tablebuttons');
    //const languageBut = document.querySelector('.nav__links a');
    //const languageDropDown = document.querySelector('.language-dropdown ul');
    //const dropHelper = document.querySelector('.dropper');
    //const safeZone = document.querySelector('.safezone');

    const tableContent = $('.tablediv');
    const tableButtons = $('.tablebuttons');

    const languageBut = $('.nav__links a');
    const languageDropDown = $('.language-dropdown ul');
    const dropHelper = $('.dropper');
    const safeZone = $('.safezone');


    function dropLan() {
        languageDropDown.addClass('dropfirst');
        languageDropDown.removeClass('dropback');
        safeZone.css("display", "inline");
        //safeZone.style.display = 'inline';
        dropHelper.css("display", "inline");
        //dropHelper.style.display = 'inline';
        //languageDropDown.style.display = 'flex';
        languageDropDown.css("display", "flex");

    }
    function hideLan() {
        safeZone.css("display", "none");
        dropHelper.css("display", "none");
        languageDropDown.removeClass('dropfirst');
        languageDropDown.addClass('dropback');
        languageDropDown.css("display", "flex");
    }

    languageBut.on('mouseover', dropLan);
    safeZone.on('mouseover', hideLan);
    //const deleteBack = document.querySelector('.backgrey');
    //const deletepopup = document.querySelector('.popup');

    const deleteBack = $('.backgrey');
    const deletepopup = $('.popup');
    const deleteAskCancelBut = $(
        '.popup ul li:first-of-type a'
    );
    const deleteAskDeleteBut = $(
        '.popup ul li:last-of-type a'
    );

    deleteBack.css("display", "none");
    //deleteBack.style.display = 'none';
    deletepopup.css("display", "none");

    //deletepopup.style.display = 'none';

    //function dropLan() {
    //    languageDropDown.classList.add('dropfirst');
    //    languageDropDown.classList.remove('dropback');
    //    safeZone.style.display = 'inline';
    //    dropHelper.style.display = 'inline';
    //    languageDropDown.style.display = 'flex';
    //}
    //function hideLan() {
    //    safeZone.style.display = 'none';
    //    dropHelper.style.display = 'none';
    //    languageDropDown.classList.remove('dropfirst');
    //    languageDropDown.classList.add('dropback');
    //    languageDropDown.style.display = 'flex';
    //}

    //languageBut.addEventListener('mouseover', dropLan);
   //safeZone.addEventListener('mouseover', hideLan);

   

    languageBut.on('mouseover', dropLan);
    safeZone.on('mouseover', hideLan);


    function popUpper() {
        deleteBack.css("display", "block");
        deletepopup.css("display", "block");
        deleteBack.removeClass("backgreyanimback");
        deleteBack.addClass('backgreyanim');
        deletepopup.removeClass("backgreyanimback");
        deletepopup.addClass('popupanim');

        ////deleteBack.classList.add('backgreyanim');
        //deletepopup.classList.remove('popupanimback');
        //deletepopup.classList.add('popupanim');
    }
    function popDowner() {
        deleteBack.removeClass('backgreyanim');
        deleteBack.classList.addClass('backgreyanimback');
        deletepopup.removeClass('popupanim');
        deletepopup.addClass('popupanimback');
        //deleteBack.classList.remove('backgreyanim');
        //deleteBack.classList.add('backgreyanimback');
        //deletepopup.classList.remove('popupanim');
        //deletepopup.classList.add('popupanimback');
        setTimeout(function () {
            deleteBack.css("display", "none");
            deletepopup.css("display", "none");
        }, 500);
    }
    //deleteAskCancelBut.addEventListener('click', popDowner);
    //deleteAskDeleteBut.addEventListener('click', popDowner);
    //deleteBack.addEventListener('click', popDowner);

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