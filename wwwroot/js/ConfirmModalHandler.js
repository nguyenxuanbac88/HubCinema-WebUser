function openConfirmModal() {
    document.getElementById("confirmPasswordModal").style.display = "flex";
    closeLoginModal(); // nếu muốn ẩn login
}
function closeConfirmModal() {
    document.getElementById("confirmPasswordModal").style.display = "none";
}
