const upBan = document.querySelector('.magnify');
const banner = document.querySelector('.banner');
const upBox = document.querySelector('.boxes');
const subsBody = document.querySelector('.subscribernum');

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
