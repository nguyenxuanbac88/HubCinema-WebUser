document.addEventListener("DOMContentLoaded", function () {
    const confirmForm = document.getElementById("confirmPasswordForm");
    const resultDiv = document.getElementById("confirmResult");

    confirmForm.addEventListener("submit", async function (e) {
        e.preventDefault();
        resultDiv.classList.add("d-none");

        const newPassword = document.getElementById("newPassword").value;

        const res = await fetch("http://api.dvxuanbac.com:2030/api/auth/confirm-password", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: window.username,
                otp: window.verifiedOtp,
                newPW: newPassword,
                otpToken: window.otpToken
            })
        });

        const data = await res.json();

        if (res.ok && data.message.includes("thành công")) {
            resultDiv.textContent = data.message;
            resultDiv.className = "alert alert-success mt-3";
        } else {
            resultDiv.textContent = data.message || "Đổi mật khẩu thất bại.";
            resultDiv.className = "alert alert-danger mt-3";
        }

        resultDiv.classList.remove("d-none");
    });

    window.openConfirmPasswordModal = () => document.getElementById("confirmPasswordModal").style.display = "flex";
    window.closeConfirmPasswordModal = () => document.getElementById("confirmPasswordModal").style.display = "none";
});
