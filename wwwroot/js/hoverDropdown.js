document.addEventListener('DOMContentLoaded', function () {
    const navbarDropdowns = document.querySelectorAll('.navbar-dropdown');

    navbarDropdowns.forEach(container => {
        const dropdown = container.querySelector('.dropdown');
        const menu = dropdown.querySelector('.dropdown-menu');

        let timeout;

        container.addEventListener('mouseenter', () => {
            clearTimeout(timeout);
            menu.style.display = 'block';
            menu.style.opacity = '1';
            menu.style.visibility = 'visible';
        });

        container.addEventListener('mouseleave', () => {
            timeout = setTimeout(() => {
                menu.style.display = 'none';
                menu.style.opacity = '0';
                menu.style.visibility = 'hidden';
            }, 200); // nhỏ delay để tránh mất menu khi rê nhanh
        });
    });
});
