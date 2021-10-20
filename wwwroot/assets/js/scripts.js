$(document).ready(function () {

    // expand function
    $('body').on('click', '.click-expand', function () {
        let this_data = $(this).attr('data-expand')
        let this_child = $(this).parent().find('.expand-block:first')

        $('.dropdown-list').slideUp(250)
        $('.dropdown-select').attr('data-expand', 'false')

        if (this_data === 'true') {
            $(this).attr('data-expand', 'false')
            this_child.slideUp(250)
            // $('.input-expand').blur()
        } else if (this_data === 'false') {
            this_child.slideDown(250)
            $(this).attr('data-expand', 'true')
        } else {
            this_child.slideUp(250)
            // $('.input-expand').blur()
        }
    })

    // nav - mobile - open / close
    $('body').on('click', '[click-burger-nav-open]', function () {
        $('[nav-mobile-open]').css({
            right: '0'
        })
        $('body').addClass('overflow-hidden')
    })
    $('body').on('click', '[click-close-nav-close]', function () {
        $('[nav-mobile-open]').css({
            right: '100%'
        })
        $('body').removeClass('overflow-hidden')
    })

    // serach - active - result - toggle
    if (window.screen.width >= 1024) {
        $('body').on('focus ', '[serach-focus-toggle]', function () {
            $('[serach-focus-result]').show()
        })
        $('body').on('focusout ', '[serach-focus-toggle]', function () {
            $('[serach-focus-result]').hide()
        })
    }
    // serach - active - result - toggle / mobile
    $('body').on('click', '[click-serach-open]', function () {
        $('[header-mobile-serach]').css({
            right: '0'
        })
        $('body').addClass('overflow-hidden')
        $('[serach-focus-toggle]').focus()
    })
    $('body').on('click', '[click-close-search-close]', function () {
        $('[header-mobile-serach]').css({
            right: '100%'
        })
        $('body').removeClass('overflow-hidden')
    })


    // change_active function
    $('body').on('click', '[data-active]', function () {
        let thic_parent = $(this).parent()
        let this_active = $(thic_parent).find('[data-active]')
        $(this_active).removeClass('active')
        $(this).addClass('active')
    })

    // catalog function

    $('body').on('click', '[click-catalog-open-close]', function () {
        $(this).toggleClass('active')
        $('[click-catalog-name]').removeClass('active')
        $('[catalog-sub-menu]').removeClass('open')
        $('body').toggleClass('overflow-hidden')
        $('[navbar-onscroll-sticky]').toggleClass('main')

        if (window.screen.width >= 768) {
            $('[catalog-item]').each(function () {
                let this_parent = $(this).parent()
                let this_childrn = $(this_parent).children()
                let first_box_show = $(this_childrn)[0]
                $(first_box_show).find('[click-catalog-name]').addClass('active')
                let catalogSubMenuFirst = $(first_box_show).find('[catalog-sub-menu]')
                catalogSubMenuFirst.addClass('open')
            })
        }

        $('[navbar-catalog-toggle]').toggleClass('open')
    })

    if (window.screen.width >= 768) {
        $('body').on('mouseover', '[click-catalog-name]', function () {
            let catalogList = $(this).parent()
            let catalogItem = $(catalogList).parent()
            let catalogItemOpen = $(catalogItem).find('[catalog-sub-menu]')
            let catalogName = $(this).parent()
            let subMenu = $(catalogName).find('[catalog-sub-menu]')
            if ($(catalogItemOpen).hasClass('open')) {
                $(catalogItemOpen).removeClass('open')
            }
            $(subMenu).addClass('open')
        })

        $('body').on('mouseover', '[change-active-catalog]', function () {
            let this_active = $('[change-active-catalog]')
            $(this_active).removeClass('active')
            $(this).addClass('active')
        })
    }


    // function for window click
    $(window).on('click', function (e) {
        if (!$(e.target).closest('[window-click]').length) {
            $('.dropdown-list').slideUp(250)
            $('.dropdown-select').attr('data-expand', 'false')
            $('[navbar-catalog-toggle]').removeClass('open')
            $('[catalog-sub-menu]').removeClass('open')
            $('[click-catalog-open-close]').removeClass('active')
            $('[navbar-onscroll-sticky]').removeClass('main')

            $('body').removeClass('overflow-hidden')
        }
    })
})


// modal overflow --in blazor--
function addClassToBody() {
    document.body.classList.add('modal-overflow')
}

function removeClassFromBody() {
    document.body.classList.remove('modal-overflow')
}



// open before release  // inspect element false

// document.onkeydown = (function (event) {
//     if (event.keyCode == 123) // Prevent F12
//     {
//         return false;
//     } else if (event.ctrlKey && event.shiftKey && event.keyCode == 73)
//     // Prevent Ctrl+Shift+I
//     {
//         return false;
//     }
// });

// document.oncontextmenu = function () {
//     return false
// }