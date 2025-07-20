document.addEventListener("DOMContentLoaded", () => {
    const carousel = document.querySelector(".news-carousel");

    if (!carousel) return;

    let scrollAmount = 0;
    const scrollStep = 260; // mỗi lần trượt ngang bao nhiêu pixel (card width + margin)
    const delay = 3000; // thời gian giữa các lần trượt (ms)

    setInterval(() => {
        if (scrollAmount + carousel.clientWidth >= carousel.scrollWidth) {
            scrollAmount = 0;
        } else {
            scrollAmount += scrollStep;
        }

        carousel.scrollTo({
            left: scrollAmount,
            behavior: 'smooth'
        });
    }, delay);
});
