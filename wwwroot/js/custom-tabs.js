document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll(".tab");
    const panels = document.querySelectorAll(".tab-panel");

    tabs.forEach(tab => {
        tab.addEventListener("click", function () {
            // Bỏ active khỏi các tab
            tabs.forEach(t => t.classList.remove("active"));
            this.classList.add("active");

            // Ẩn tất cả panel
            panels.forEach(p => p.classList.remove("show"));

            // Hiện panel được chọn
            const target = this.getAttribute("data-target");
            const panel = document.querySelector(target);
            if (panel) {
                panel.classList.add("show");
            }
        });
    });
});
