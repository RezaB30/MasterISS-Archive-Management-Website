window.onscroll = function () {
  myFunction();
};

var archiveNumber = document.getElementById('archiveNum');
var sticky = archiveNumber.offsetTop;

function myFunction() {
  if (window.pageYOffset >= sticky + 30) {
    archiveNumber.style.position = 'none';
    archiveNumber.classList.add('sticky');
  } else {
    archiveNumber.style.position = 'sticky';
    archiveNumber.classList.remove('sticky');
  }
}
