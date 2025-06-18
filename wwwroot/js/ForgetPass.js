document.addEventListener("DOMContentLoaded", function () {
    window.openForgotModal = function () {
        const forgotModal = document.getElementById("forgotModal");
        if (forgotModal) {
            forgotModal.style.display = "flex";
            closeLoginModal();
            closeRegisterModal();
        }
    };

    window.closeForgotModal = function () {
        const forgotModal = document.getElementById("forgotModal");
        if (forgotModal) {
            forgotModal.style.display = "none";
        }
    };
});
