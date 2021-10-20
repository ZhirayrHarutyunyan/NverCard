window.phoneMask = {
   init: function () {
      document.querySelector('[data-phone-mask]').onkeypress = (e) => {
         let numValLength = document.querySelector('[data-phone-mask]').value.length
         if (numValLength == 0) {
            document.querySelector('[data-phone-mask]').value += "("
         }
         if (numValLength == 3) {
            document.querySelector('[data-phone-mask]').value += ") "
         }
         if (numValLength == 7 || numValLength == 10) {
            document.querySelector('[data-phone-mask]').value += "-"
         }
         document.querySelector('[data-phone-mask]').setAttribute("maxlength", "13")
         keys = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
         return keys.indexOf(e.key) > -1
      }
   }
}