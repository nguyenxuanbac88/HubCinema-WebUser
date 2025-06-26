document.addEventListener("DOMContentLoaded", function () {
    const mobileMenu = document.getElementById('mobileMenu');
    const menuToggle = document.getElementById('menuToggle');
    const menuClose = document.getElementById('menuClose');

    if (menuToggle && mobileMenu && menuClose) {
        menuToggle.addEventListener('click', () => {
            mobileMenu.style.transform = 'translateX(0)';
        });

        menuClose.addEventListener('click', () => {
            mobileMenu.style.transform = 'translateX(100%)';
        });
    }
});
