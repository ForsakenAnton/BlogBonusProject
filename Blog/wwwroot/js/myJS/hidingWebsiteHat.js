
let elY = 0;
let scrollY = 0;

window.addEventListener("scroll", function () {
    const el = document.getElementById('header');
    const height = el.offsetHeight;
    const pos = window.pageYOffset;
    const diff = scrollY - pos;

    elY = Math.min(0, Math.max(-height, elY + diff));
    el.style.position = pos >= height ? 'fixed' : pos === 0 ? '' : el.style.position;
    el.style.transform = `translateY(${el.style.position === 'fixed' ? elY : 0}px)`;

    scrollY = pos;
});










//window.onload = function () {
//    var header = document.getElementById("header");
//    var main = document.getElementById("main");
//    var headerHeight = header.offsetHeight;
//    main.style.marginTop = headerHeight + "px";

//    window.onscroll = function () {
//        if (document.body.scrollTop > headerHeight || document.documentElement.scrollTop > headerHeight) {
//            header.style.display = 'none';
//            main.style.marginTop = 0;
//        } else {
//            header.style.display = 'block';
//            main.style.marginTop = headerHeight + "px";
//        }
//    }
//}

//var prevScrollpos = window.pageYOffset;

//window.onscroll = function () {
//    var currentScrollPos = window.pageYOffset;

//    if (prevScrollpos > currentScrollPos) {
//        document.getElementById("header").style.top = "0";
//    }
//    else {
//        document.getElementById("header").style.top = "-" + document.getElementById("header").offsetHeight + "px";
//    }

//    prevScrollpos = currentScrollPos;
//}