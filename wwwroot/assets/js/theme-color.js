let switchColor = document.querySelectorAll('[data-switch-color]')
switchColor.forEach(function (item) {
   item.addEventListener('click', function () {
      if (item.getAttribute('data-color-mode') == 'dark') {
         document.documentElement.setAttribute('data-theme-mode', 'dark')
         localStorage.setItem('theme', 'dark')
      } else if (item.getAttribute('data-color-mode') == 'light') {
         document.documentElement.setAttribute('data-theme-mode', 'light')
         localStorage.setItem('theme', 'light')
      } else if (item.getAttribute('data-color-mode') == 'default') {
         document.documentElement.removeAttribute('data-theme-mode')
         localStorage.setItem('theme', '')
      }
   })
})

if (localStorage.getItem('theme') == 'dark') {
   document.documentElement.setAttribute('data-theme-mode', 'dark')
   document.querySelector('[data-color-mode="dark"]').classList.add('active')
   document.querySelector('[data-color-mode="default"]').classList.remove('active')
} else if (localStorage.getItem('theme') == 'light') {
   document.documentElement.setAttribute('data-theme-mode', 'light')
   document.querySelector('[data-color-mode="light"]').classList.add('active')
   document.querySelector('[data-color-mode="default"]').classList.remove('active')
}

document.querySelector('[data-theme-btn]').addEventListener('click', function () {
   document.querySelector('[data-theme').classList.toggle('active')
})