﻿function togglePassword(fieldId, iconSpan) {
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