document.addEventListener("DOMContentLoaded", function () {
    const otpForm = document.getElementById("otpCheckForm");
    const errorDiv = document.getElementById("otpError");

    otpForm.addEventListener("submit", async function (e) {
        e.preventDefault();
        errorDiv.classList.add("d-none");

        const otp = document.getElementById("inputOtp").value;

        const res = await fetch("http://api.dvxuanbac.com:2030/api/auth/check-otp", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: window.username,
                otp: otp,
                otpToken: window.otpToken
            })
        });

        const data = await res.json();

        if (res.ok && data.message.includes("hợp lệ")) {
            window.verifiedOtp = otp;
            closeOTPCheckModal();
            openConfirmPasswordModal();
        } else {
            errorDiv.textContent = data.message || "OTP không đúng.";
            errorDiv.classList.remove("d-none");
        }
    });

    window.openOTPCheckModal = () => document.getElementById("otpCheckModal").style.display = "flex";
    window.closeOTPCheckModal = () => document.getElementById("otpCheckModal").style.display = "none";
});
