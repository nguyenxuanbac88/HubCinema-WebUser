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
function togglePassword(fieldId, iconSpan) {
    const input = document.getElementById(fieldId);
    const icon = iconSpan.querySelector('i');
    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("bi-eye-slash");
        icon.classList.add("bi-eye");
    } else {
        input.type = "password";
        icon.classList.remove("bi-eye");
        icon.classList.add("bi-eye-slash");
    }
}