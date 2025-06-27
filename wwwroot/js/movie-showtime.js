document.addEventListener("DOMContentLoaded", function () {
    const container = document.getElementById("date-buttons");
    if (!container) return;

    container.querySelectorAll("button").forEach(btn => {
        btn.addEventListener("click", function () {
            // Bỏ active của các button khác
            container.querySelectorAll("button").forEach(b => {
                b.classList.remove("btn-primary", "text-white");
                b.classList.add("btn-outline-secondary");
            });

            // Thêm active cho button được click
            btn.classList.remove("btn-outline-secondary");
            btn.classList.add("btn-primary", "text-white");

            // OPTIONAL: submit form hoặc trigger sự kiện nếu muốn gửi request từ controller
            // document.getElementById("yourForm").submit();
        });
    });
});
