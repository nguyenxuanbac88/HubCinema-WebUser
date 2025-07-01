
    document.getElementById("otpForm").addEventListener("submit", async function (e) {
        e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);

    const response = await fetch(form.action, {
        method: "POST",
    body: formData
        });

    const result = await response.json();

    const errorDiv = document.getElementById("otpError");
    const successDiv = document.getElementById("otpSuccess");

    if (result.success) {
        errorDiv.style.display = "none";
    successDiv.textContent = result.message;
    successDiv.style.display = "block";

            // Ẩn modal OTP sau 1 giây và mở modal đặt lại mật khẩu
            setTimeout(() => {
        closeOtpModal();

    if (typeof openConfirmPasswordModal === "function") {
        openConfirmPasswordModal();
                }
            }, 1000);
        } else {
        successDiv.style.display = "none";
    errorDiv.textContent = result.message;
    errorDiv.style.display = "block";
        }
    });

    

