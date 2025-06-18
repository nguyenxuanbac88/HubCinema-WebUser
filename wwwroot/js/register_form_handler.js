document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('#registerModal form');
    if (!form) return;

    form.addEventListener('submit', function () {
        const day = document.getElementById('dob_day').value;
        const month = document.getElementById('dob_month').value;
        const year = document.getElementById('dob_year').value;

        if (day && month && year) {
            const dob = `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
            document.getElementById('dobHiddenInput').value = dob;
        }
    });
});
