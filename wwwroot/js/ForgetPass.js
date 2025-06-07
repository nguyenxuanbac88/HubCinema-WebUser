function openForgotModal() {
    document.getElementById("forgotModal").style.display = "flex";
    closeLoginModal();
    closeRegisterModal();
}
function closeForgotModal() {
    document.getElementById("forgotModal").style.display = "none";
}