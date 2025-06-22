$(document).ready(function () {
    $(".banner-slider").slick({
        slidesToShow: 1,
        centerMode: true,
        centerPadding: "12%", // để 1 phần pre và post banner lộ ra
        dots: true,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 4000,
        pauseOnHover: true,
        prevArrow: '<button class="slick-prev">&#10094;</button>',
        nextArrow: '<button class="slick-next">&#10095;</button>',
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