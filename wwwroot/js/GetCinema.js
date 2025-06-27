document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll(".tab-date");

    tabs.forEach(tab => {
        tab.addEventListener("click", function () {
            // Xoá class active khỏi tất cả
            tabs.forEach(t => t.classList.remove("active"));

            // Thêm class active vào tab đang click
            this.classList.add("active");
        });
    });
});