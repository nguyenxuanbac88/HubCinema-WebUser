function openRegisterModal() {
    document.getElementById("registerModal").style.display = "flex";
    closeLoginModal();
    closeForgotModal();
}
function closeRegisterModal() {
    document.getElementById("registerModal").style.display = "none";
}
function switchToLoginFromRegister() {
    document.getElementById("registerModal").style.display = "none";
    document.getElementById("loginModal").style.display = "flex";
}


