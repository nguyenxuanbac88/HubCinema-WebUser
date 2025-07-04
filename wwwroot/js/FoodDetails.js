document.addEventListener("DOMContentLoaded", function () {
    const quantityInput = document.getElementById("quantityInput");
    const btnMinus = document.getElementById("btnMinus");
    const btnPlus = document.getElementById("btnPlus");

    btnMinus.addEventListener("click", function () {
        let val = parseInt(quantityInput.value);
        if (val > 1) quantityInput.value = val - 1;
    });

    btnPlus.addEventListener("click", function () {
        let val = parseInt(quantityInput.value);
        quantityInput.value = val + 1;
    });
});