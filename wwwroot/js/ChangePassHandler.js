document.addEventListener("DOMContentLoaded", function () {
    const message = '@(TempData["ChangePasswordMessage"] ?? "").Trim()';
    if (message !== '') {
        // Hiển thị thông báo dạng toast hoặc alert nếu muốn
        console.log("ChangePasswordMessage:", message);
        // KHÔNG mở modal ở đây
    }
});
function openChangePasswordModal() {
    var modal = document.getElementById("changePasswordModal");
    if (modal) modal.style.display = "flex";
}

function closeChangePasswordModal() {
    var modal = document.getElementById("changePasswordModal");
    if (modal) modal.style.display = "none";
}

