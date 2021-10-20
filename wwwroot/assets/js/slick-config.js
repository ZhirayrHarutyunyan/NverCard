window.slickSliderConfigs = {
   init: function () {
      // slickConfigBannerSlider
      $('[banner-slider]').slick({
         slidesToShow: 1,
         slidesToScroll: 1,
         variableWidth: true,
         arrows: false,
         dots: true,
         autoplay: true,
         responsive: [{
            breakpoint: 767.98,
            settings: {
               variableWidth: false,
            },
         }, ],
      });
      // slickConfigProductSlider
      $('[product-slider]').slick({
         slidesToShow: 5,
         slidesToScroll: 1,
         variableWidth: true,
         responsive: [{
            breakpoint: 767.98,
            settings: {
               slidesToShow: 1,
               slidesToScroll: 1,
               centerMode: true,
               arrows: false,
               dots: true,
               dotsClass: 'slick-dots-counter',
               customPaging: function (slider, i) {
                  return (i + 1) + '/' + slider.slideCount;
               }
            },
         }, ],
      });
      // slickConfigCategorySlider
      $('[category-slider]').slick({
         slidesToShow: 4,
         slidesToScroll: 1,
         variableWidth: true,
         arrows: false,
         dots: true,
         responsive: [{
            breakpoint: 767.98,
            settings: {
               slidesToShow: 1,
               slidesToScroll: 1,
               centerMode: true,
            },
         }, ],
      });
   }
}