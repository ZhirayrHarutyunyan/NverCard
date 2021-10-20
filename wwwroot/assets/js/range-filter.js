window.rangeFilter = {
   init: function () {
      let
         rangeS = document.querySelectorAll('[data-type-range]'),
         getSetNumberVelue = document.querySelectorAll('[get-set-number-velue]'),
         circle = document.querySelectorAll('[data-type-circle]'),
         line = document.querySelector('[data-type-line]')

      rangeS.forEach(function (el) {
         let rangeWidth1 = (el.offsetWidth - circle[0].offsetWidth) / (parseInt(rangeS[0].max) - parseInt(rangeS[0].min)),
            rangeWidth2 = (el.offsetWidth - circle[1].offsetWidth) / (parseInt(rangeS[1].min) - parseInt(rangeS[1].max))
         el.oninput = function () {
            let slide1 = parseInt(rangeS[0].value),
               slide2 = parseInt(rangeS[1].value)
            let pxValue1 = ((slide1 - parseInt(rangeS[0].min)) * rangeWidth1) - (circle[0].offsetWidth / 100),
               pxValue2 = ((slide2 - parseInt(rangeS[1].max)) * rangeWidth2) - (circle[1].offsetWidth / 100)
            if (slide1 > slide2 - 5000) {
               slide1 - 5000
               return false
            }
            getSetNumberVelue[0].innerHTML = slide1
            getSetNumberVelue[1].innerHTML = slide2
            circle[0].style.left = pxValue1 + 'px'
            circle[1].style.right = pxValue2 + 'px'
            line.style.left = pxValue1 + 'px'
            line.style.right = pxValue2 + 'px'
         }
      })
   }
}