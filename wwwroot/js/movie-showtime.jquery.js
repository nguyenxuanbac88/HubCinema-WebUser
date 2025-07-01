$(document).ready(function () {
    const $dateButtons = $("#date-buttons");
    const $regionSelect = $("#region-select");
    const $cinemaSelect = $("#cinema-select");
    const $showtimeContainer = $("#showtime-list");
    const allShowtimesJson = $("#all-showtimes-data").text();

    if (!$dateButtons.length || !$showtimeContainer.length || !allShowtimesJson) return;

    const allShowtimes = JSON.parse(allShowtimesJson);

    let selectedDate = $dateButtons.find(".btn-primary").data("date");
    let selectedRegion = $regionSelect.val();
    let selectedCinema = $cinemaSelect.val();

    function renderShowtimes() {
        const theaters = allShowtimes[selectedDate] || [];
        let html = "";

        const filtered = theaters.filter(t => {
            const matchRegion = !selectedRegion || t.region === selectedRegion;
            const matchCinema = !selectedCinema || t.cinemaId.toString() === selectedCinema;
            return matchRegion && matchCinema;
        });

        if (filtered.length === 0) {
            $showtimeContainer.html(`<div class="text-center text-muted py-4">Không có lịch chiếu cho lựa chọn này.</div>`);
            return;
        }

        $.each(filtered, function (i, theater) {
            const bg = i % 2 === 0 ? "bg-light" : "bg-white";
            html += `<div class="py-4 px-3 ${bg} border-top border-bottom mb-3">
                        <h6 class="fw-bold mb-3">${theater.theaterName}</h6>
                        <div class="d-flex flex-wrap gap-2">`;

            $.each(theater.showtimes, function (_, s) {
                const time = new Date(s.startTime).toLocaleTimeString([], {
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: false
                });
                html += `<button class="btn btn-outline-primary btn-sm">${time}</button>`;
            });

            html += `</div></div>`;
        });

        $showtimeContainer.html(html);
    }

    // Xử lý chọn ngày
    $dateButtons.on("click", "button", function () {
        $dateButtons.find("button").removeClass("btn-primary text-white").addClass("btn-outline-secondary");
        $(this).addClass("btn-primary text-white").removeClass("btn-outline-secondary");

        selectedDate = $(this).data("date");
        renderShowtimes();
    });

    // Xử lý chọn region
    $regionSelect.on("change", function () {
        selectedRegion = $(this).val();

        $cinemaSelect.find("option").each(function () {
            const $opt = $(this);
            const region = $opt.data("region");
            if (!$opt.val()) return; // giữ "Tất cả rạp"
            const visible = !selectedRegion || region === selectedRegion;
            $opt.prop("hidden", !visible);
        });

        // Reset nếu rạp đang chọn không thuộc region mới
        if ($cinemaSelect.find("option:selected").prop("hidden")) {
            $cinemaSelect.val("");
            selectedCinema = "";
        }

        renderShowtimes();
    });

    // Xử lý chọn cinema
    $cinemaSelect.on("change", function () {
        selectedCinema = $(this).val();
        renderShowtimes();
    });

    // Gọi ban đầu
    renderShowtimes();
});
