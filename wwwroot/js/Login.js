function openLoginModal() {
    document.getElementById("loginModal").style.display = "flex";
    closeRegisterModal();
    closeForgotModal();
}
function closeLoginModal() {
    document.getElementById("loginModal").style.display = "none";
}
function toggleLoginPassword(iconSpan) {
    const input = document.getElementById("loginPassword");
    const icon = iconSpan.querySelector("i");

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
document.addEventListener("DOMContentLoaded", function () {
    const showLogin = '@TempData["ShowLoginModal"]';
    if (showLogin === 'True') {
        const modal = document.getElementById("loginModal");
        if (modal) modal.style.display = "block";
    }
});