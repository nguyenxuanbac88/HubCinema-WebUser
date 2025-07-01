// ✅ Hàm mở modal: đã đúng
window.openConfirmPasswordModal = function () {
    const modal = document.getElementById('confirmPasswordModal');
    if (modal) {
        modal.style.display = 'flex';
    } else {
        console.warn('Không tìm thấy confirmPasswordModal');
    }
};

// ✅ Gán hàm đóng modal vào window
window.closeConfirmPasswordModal = function () {
    const modal = document.getElementById('confirmPasswordModal');
    if (modal) {
        modal.style.display = 'none';
        const pwInput = modal.querySelector('input[name="newPassword"]');
        if (pwInput) pwInput.value = '';
    }
};

// ✅ Xử lý bấm ra ngoài để đóng modal
document.addEventListener('click', function (event) {
    const modal = document.getElementById('confirmPasswordModal');
    if (modal && modal.style.display === 'block' && event.target === modal) {
        window.closeConfirmPasswordModal();
    }
});
