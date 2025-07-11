document.addEventListener("DOMContentLoaded", function () {
    const tabButtons = document.querySelectorAll(".tab-date");
    const movieCards = document.querySelectorAll(".movie-card");

    if (tabButtons.length > 0) {
        const firstTab = tabButtons[0];
        const defaultDate = firstTab.getAttribute("data-date");

        // 1. Hiển thị ngày mặc định (tab đầu tiên)
        filterCardsByDate(defaultDate);
        tabButtons.forEach(btn => btn.classList.remove("active"));
        firstTab.classList.add("active");

        // 2. Gắn sự kiện click cho từng tab
        tabButtons.forEach(button => {
            button.addEventListener("click", () => {
                const date = button.getAttribute("data-date");

                tabButtons.forEach(btn => btn.classList.remove("active"));
                button.classList.add("active");

                filterCardsByDate(date);
            });
        });
    }

    function filterCardsByDate(date) {
        movieCards.forEach(card => {
            const cardDate = card.getAttribute("data-date");
            card.style.display = (cardDate === date) ? "block" : "none";
        });
    }
});
