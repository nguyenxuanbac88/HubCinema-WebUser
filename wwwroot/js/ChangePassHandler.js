document.addEventListener("DOMContentLoaded", function () {
    const message = '@(TempData["ChangePasswordMessage"] ?? "").Trim()';
    if (message !== '') {
        openChangePasswordModal();
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

