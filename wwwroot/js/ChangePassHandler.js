document.addEventListener("DOMContentLoaded", function () {
    // Các hàm xử lý modal ở đây
    window.openChangePasswordModal = function () {
        const modal = document.getElementById("changePasswordModal");
        if (modal) modal.style.display = "flex";
    };

    window.closeChangePasswordModal = function () {
        const modal = document.getElementById("changePasswordModal");
        if (modal) modal.style.display = "none";
    };
});
