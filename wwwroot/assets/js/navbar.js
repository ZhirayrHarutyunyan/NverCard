window.navbarFixed = {
   init: function () {
      if (window.screen.width >= 768) {
         window.onscroll = function () {
            navbarScroll()
         };
         let navbar = document.querySelector('[navbar-onscroll-sticky]'),
            navbarToggleLogo = document.querySelector('[navbar-onscroll-sticky-logo]'),
            navbarToggleSearch = document.querySelector('[navbar-onscroll-sticky-search]'),
            searchScrollToggle = document.querySelector('[header-mobile-serach]'),
            sticky = navbar.offsetTop;

         function navbarScroll() {
            if (window.pageYOffset > sticky) {
               navbarToggleLogo.classList.remove("d-none")
               navbarToggleLogo.classList.add("d-block")
               navbarToggleSearch.classList.remove("d-none")
               navbarToggleSearch.classList.add("d-block")
               searchScrollToggle.style.display = 'none'
            } else {
               navbarToggleLogo.classList.remove("d-block")
               navbarToggleLogo.classList.add("d-none")
               navbarToggleSearch.classList.remove("d-block")
               navbarToggleSearch.classList.add("d-none")
               searchScrollToggle.style.display = 'block'
            }
         }
      }
   }
}

// nav-scroll down -- mobile
// if (window.screen.width < 768) {
//    let navbar = document.querySelector('[navbar-onscroll-sticky]')
//    let scrollPos = 0
//    window.addEventListener('scroll', function () {
//       if ((document.body.getBoundingClientRect()).top > scrollPos) {
//          navbar.classList.remove('down')
//       } else {
//          navbar.classList.add('down')
//       }
//       scrollPos = (document.body.getBoundingClientRect()).top
//    });
// }