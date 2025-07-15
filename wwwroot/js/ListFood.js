
    document.addEventListener("DOMContentLoaded", function () {
        const toggleBtn = document.getElementById("toggleFoodBtn");
    if (!toggleBtn) return;

    const foodItems = document.querySelectorAll(".food-item");
    const toggleText = toggleBtn.querySelector(".toggle-text");
    const toggleIcon = toggleBtn.querySelector(".toggle-icon");

    let isExpanded = false;

    toggleBtn.addEventListener("click", function () {
        isExpanded = !isExpanded;

            foodItems.forEach((el, index) => {
                if (index >= 4) {
        el.style.display = isExpanded ? "block" : "none";
                }
            });

    toggleText.textContent = isExpanded ? "Thu gọn" : "Xem thêm";
    toggleIcon.style.transform = isExpanded ? "rotate(90deg)" : "rotate(0)";
        });
    });

