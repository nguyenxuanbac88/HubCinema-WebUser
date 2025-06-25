document.addEventListener('DOMContentLoaded', function () {
    const dropdownItems = document.querySelectorAll('.navbar .dropdown');

    dropdownItems.forEach(dropdown => {
        const toggle = dropdown.querySelector('.nav-link');
        const menu = dropdown.querySelector('.dropdown-menu');

        // Bỏ thuộc tính Bootstrap để không bị can thiệp
        toggle.removeAttribute('data-bs-toggle');

        // Gán hiệu ứng
        dropdown.addEventListener('mouseenter', () => {
            menu.classList.add('show');
            toggle.classList.add('show');
        });

        dropdown.addEventListener('mouseleave', () => {
            menu.classList.remove('show');
            toggle.classList.remove('show');
        });
    });
});
