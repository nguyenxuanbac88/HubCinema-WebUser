document.addEventListener("DOMContentLoaded", function () {
    const dateButtons = document.getElementById("date-buttons");
    const regionSelect = document.querySelector("select#region-select");
    const cinemaSelect = document.querySelector("select#cinema-select");
    const showtimeContainer = document.querySelector(".movie_showtime .mt-4");
    const allShowtimesJson = document.getElementById("all-showtimes-data")?.textContent;

    if (!dateButtons || !showtimeContainer || !allShowtimesJson) return;

    const allShowtimes = JSON.parse(allShowtimesJson);

    let selectedDate = dateButtons.querySelector(".btn-primary")?.getAttribute("data-date");
    let selectedRegion = "";
    let selectedCinema = "";

    function renderShowtimes() {
        const theaters = allShowtimes[selectedDate] || [];
        let html = "";

        const filteredTheaters = theaters.filter(theater => {
            const matchRegion = !selectedRegion || theater.region === selectedRegion;
            const matchCinema = !selectedCinema || theater.cinemaId.toString() === selectedCinema;
            return matchRegion && matchCinema;
        });

        if (filteredTheaters.length === 0) {
            showtimeContainer.innerHTML = `<div class="text-center text-muted py-4">Không có lịch chiếu cho lựa chọn này.</div>`;
            return;
        }

        filteredTheaters.forEach((theater, index) => {
            const bgClass = index % 2 === 0 ? "bg-light" : "bg-white";
            html += `<div class="py-4 px-3 ${bgClass} border-top border-bottom mb-3">
                        <h6 class="fw-bold mb-3">${theater.theaterName}</h6>
                        <div class="d-flex flex-wrap gap-2">`;

            (theater.showtimes || []).forEach(show => {
                const time = new Date(show.startTime).toLocaleTimeString([], {
                    hour: '2-digit', minute: '2-digit', hour12: false
                });
                html += `<button class="btn btn-outline-primary btn-sm">${time}</button>`;
            });

            html += `</div></div>`;
        });

        showtimeContainer.innerHTML = html;
    }

    // Chọn ngày
    dateButtons.querySelectorAll("button").forEach(btn => {
        btn.addEventListener("click", () => {
            dateButtons.querySelectorAll("button").forEach(b => {
                b.classList.remove("btn-primary", "text-white");
                b.classList.add("btn-outline-secondary");
            });

            btn.classList.add("btn-primary", "text-white");
            btn.classList.remove("btn-outline-secondary");

            selectedDate = btn.getAttribute("data-date");
            renderShowtimes();
        });
    });

    // Chọn vùng (region)
    regionSelect?.addEventListener("change", () => {
        selectedRegion = regionSelect.value;

        // Ẩn rạp không thuộc vùng đã chọn
        Array.from(cinemaSelect.options).forEach(option => {
            if (!option.value) return; // bỏ qua "Tất cả rạp"
            const region = option.dataset.region;
            option.hidden = selectedRegion && region !== selectedRegion;
        });

        // Reset nếu rạp đang chọn không nằm trong vùng mới
        if (cinemaSelect.selectedOptions[0]?.hidden) {
            cinemaSelect.value = "";
            selectedCinema = "";
        }

        renderShowtimes();
    });

    // Chọn rạp
    cinemaSelect?.addEventListener("change", () => {
        selectedCinema = cinemaSelect.value;
        renderShowtimes();
    });

    // Render lần đầu
    renderShowtimes();
});
