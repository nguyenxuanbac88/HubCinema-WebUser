$(document).ready(function () {
    $(".banner-slider").slick({
        slidesToShow: 1,
        centerMode: true,
        centerPadding: "12%",
        dots: true,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 4000,
        pauseOnHover: true,
        prevArrow: '<button class="slick-prev"><i class="bi bi-chevron-left"></i></button>',
        nextArrow: '<button class="slick-next"><i class="bi bi-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 768,
                settings: {
                    centerPadding: "0",
                    arrows: false,
                },
            },
        ],
    });
});
