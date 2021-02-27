const upBan = document.querySelector('.magnify');
const banner = document.querySelector('.banner');
const upBox = document.querySelector('.boxes');
const tableContent = document.querySelector('.tablediv');
const tableButtons = document.querySelector('.tablebuttons');
const languageBut = document.querySelector('.nav__links a');
const languageDropDown = document.querySelector('.language-dropdown ul');
const dropHelper = document.querySelector('.dropper');
const safeZone = document.querySelector('.safezone');
const subsBody = document.querySelector('.subscribernum');
const deleteBack = document.querySelector('.backgrey');
const deletepopup = document.querySelector('.popup');
const deleteAskCancelBut = document.querySelector(
  '.popup ul li:first-of-type a'
);
const deleteAskDeleteBut = document.querySelector(
  '.popup ul li:last-of-type a'
);

deleteBack.style.display = 'none';
deletepopup.style.display = 'none';

upBan.addEventListener('click', addUp);
upBan.addEventListener('click', function (event) {
  event.preventDefault();
});
function addUp() {
  if (banner.classList.contains('upbanner') != true) {
    setInterval(function () {
      subsBody.style.display = 'flex';
      tableContent.style.display = 'flex';
      tableButtons.style.display = 'flex';
      tableContent.classList.add('fadertable');
      tableButtons.classList.add('fadertable');
      subsBody.classList.add('subscriberfader');
    }, 500);
    banner.classList.add('upbanner');
    upBox.classList.add('upbox', 'boxgradient');
  }
}

function dropLan() {
  languageDropDown.classList.add('dropfirst');
  languageDropDown.classList.remove('dropback');
  safeZone.style.display = 'inline';
  dropHelper.style.display = 'inline';
  languageDropDown.style.display = 'flex';
}
function hideLan() {
  safeZone.style.display = 'none';
  dropHelper.style.display = 'none';
  languageDropDown.classList.remove('dropfirst');
  languageDropDown.classList.add('dropback');
  languageDropDown.style.display = 'flex';
}

languageBut.addEventListener('mouseover', dropLan);
safeZone.addEventListener('mouseover', hideLan);

function popUpper() {
  deleteBack.style.display = 'block';
  deletepopup.style.display = 'block';
  deleteBack.classList.remove('backgreyanimback');
  deleteBack.classList.add('backgreyanim');
  deletepopup.classList.remove('popupanimback');
  deletepopup.classList.add('popupanim');
}
function popDowner() {
  deleteBack.classList.remove('backgreyanim');
  deleteBack.classList.add('backgreyanimback');
  deletepopup.classList.remove('popupanim');
  deletepopup.classList.add('popupanimback');
  setTimeout(function () {
    deleteBack.style.display = 'none';
    deletepopup.style.display = 'none';
  }, 500);
}
deleteAskCancelBut.addEventListener('click', popDowner);
deleteAskDeleteBut.addEventListener('click', popDowner);
deleteBack.addEventListener('click', popDowner);

function checkForDeleteBut() {
  if (event.target.classList.contains('deletebutton')) {
    console.log(48);
    popUpper();
  }
}
document.addEventListener('click', checkForDeleteBut);
