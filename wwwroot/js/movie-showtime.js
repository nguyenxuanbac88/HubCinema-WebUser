document.addEventListener("DOMContentLoaded", function () {
    const allShowtimesEl = document.getElementById("all-showtimes-data");
    if (!allShowtimesEl) return;

    const allShowtimes = JSON.parse(allShowtimesEl.textContent);
    const showtimeContainer = document.getElementById("showtime-list");
    const dateButtons = document.querySelectorAll("#date-buttons button");
    const regionSelect = document.getElementById("region-select");
    const cinemaSelect = document.getElementById("cinema-select");

    function renderShowtimes(showtimes) {
        if (!showtimes.length) {
            showtimeContainer.innerHTML = '<div class="text-muted">Không có suất chiếu nào.</div>';
            return;
        }

        const grouped = {};
        showtimes.forEach(item => {
            const key = `${item.tenRap}__${item.maRap}`;
            if (!grouped[key]) grouped[key] = [];
            grouped[key].push(...item.gioChieu);
        });

        let html = '';
        for (const [key, gioChieuList] of Object.entries(grouped)) {
            const [tenRap] = key.split('__');
            html += `
                <div class="mb-4">
                    <h6 class="fw-bold">${tenRap}</h6>
                    <div class="d-flex flex-wrap gap-2">
                        ${gioChieuList.map(g => `<span class="badge bg-secondary">${g}</span>`).join("")}
                    </div>
                </div>
            `;
        }

        showtimeContainer.innerHTML = html;
    }

    function applyFilters() {
        const selectedDate = document.querySelector("#date-buttons button.btn-primary")?.dataset.date;
        const selectedRegion = regionSelect.value;
        const selectedCinema = cinemaSelect.value;

        const filtered = allShowtimes.filter(item => {
            const matchDate = item.date === selectedDate;
            const matchRegion = selectedRegion
                ? item.regions && item.regions.includes(selectedRegion)
                : true;
            const matchCinema = selectedCinema ? item.maRap == selectedCinema : true;
            return matchDate && matchRegion && matchCinema;
        });

        renderShowtimes(filtered);
    }


    // Ngày
    dateButtons.forEach(btn => {
        btn.addEventListener("click", () => {
            dateButtons.forEach(b => b.classList.remove("btn-primary", "text-white"));
            btn.classList.add("btn-primary", "text-white");
            btn.classList.remove("btn-outline-secondary");
            applyFilters();
        });
    });

    // Region
    regionSelect.addEventListener("change", () => {
        const selectedRegion = regionSelect.value;

        Array.from(cinemaSelect.options).forEach(opt => {
            const optRegion = opt.getAttribute("data-region");
            opt.hidden = selectedRegion && optRegion !== selectedRegion;
        });

        const currentCinema = cinemaSelect.value;
        const isVisible = cinemaSelect.querySelector(`option[value="${currentCinema}"]:not([hidden])`);
        if (!isVisible) cinemaSelect.value = "";

        applyFilters();
    });

    // Rạp
    cinemaSelect.addEventListener("change", applyFilters);

    // Lần đầu
    applyFilters();
});
