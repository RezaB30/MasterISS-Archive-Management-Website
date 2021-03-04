function SetLanguage() {

    const languageBut = $('.nav__links #language-drop');
    const languageDropDown = $('.language-dropdown ul');
    const dropHelper = $('.dropper');
    const safeZone = $('.safezone');


    function dropLan() {
        languageDropDown.addClass('dropfirst');
        languageDropDown.removeClass('dropback');
        safeZone.css("display", "inline");
        dropHelper.css("display", "inline");      
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
}