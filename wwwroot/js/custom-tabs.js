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

document.addEventListener("DOMContentLoaded", function () {
    const showMoreBtn = document.getElementById("showMoreBtn");
    if (!showMoreBtn) return;

    const movieItems = document.querySelectorAll(".movie-hidden");
    let isExpanded = false;

    showMoreBtn.addEventListener("click", function () {
        isExpanded = !isExpanded;

        movieItems.forEach(el => {
            el.style.display = isExpanded ? "block" : "none";
        });

        // Thay đổi nội dung nút và icon
        showMoreBtn.innerHTML = isExpanded
            ? `Thu gọn
                <svg aria-hidden="true" focusable="false" data-prefix="fas" data-icon="angle-right"
                    class="svg-inline--fa fa-angle-right"
                    xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 512" width="14" height="14"
                    style="transform: rotate(90deg); transition: transform 0.3s;">
                    <path fill="currentColor"
                        d="M278.6 233.4c12.5 12.5 12.5 32.8 0 45.3l-160 160c-12.5 12.5-32.8 12.5-45.3 0
                        s-12.5-32.8 0-45.3L210.7 256 73.4 118.6c-12.5-12.5-12.5-32.8
                        0-45.3s32.8-12.5 45.3 0l160 160z"></path>
                </svg>`
            : `Xem thêm
                <svg aria-hidden="true" focusable="false" data-prefix="fas" data-icon="angle-right"
                    class="svg-inline--fa fa-angle-right"
                    xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 512" width="14" height="14"
                    style="transform: rotate(0deg); transition: transform 0.3s;">
                    <path fill="currentColor"
                        d="M278.6 233.4c12.5 12.5 12.5 32.8 0 45.3l-160 160c-12.5 12.5-32.8 12.5-45.3 0
                        s-12.5-32.8 0-45.3L210.7 256 73.4 118.6c-12.5-12.5-12.5-32.8
                        0-45.3s32.8-12.5 45.3 0l160 160z"></path>
                </svg>`;
    });
});

