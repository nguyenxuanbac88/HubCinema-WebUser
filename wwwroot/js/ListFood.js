const toggleBtn = document.getElementById("toggleFoodBtn");
const toggleText = toggleBtn.querySelector(".toggle-text");

let expanded = false;

toggleBtn.addEventListener("click", function () {
    document.querySelectorAll(".food-hidden").forEach(el => {
        el.style.display = expanded ? "none" : "block";
    });

    toggleText.textContent = expanded ? "Xem thêm" : "Rút gọn";
    expanded = !expanded;
});
