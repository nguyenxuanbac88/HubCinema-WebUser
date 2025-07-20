document.addEventListener("DOMContentLoaded", async () => {
    const seatMatrix = document.getElementById("seat-matrix");
    const idSuatChieu = seatMatrix.getAttribute('data-id-suat-chieu');

    const tokenRes = await fetch("/Login/GetJwt");
    if (!tokenRes.ok) return alert("Không lấy được token");

    const { token: jwtToken } = await tokenRes.json();
    const apiUrl = `http://api.dvxuanbac.com:2030/api/Seat/get-layout-price/${idSuatChieu}`;
    const response = await fetch(apiUrl, {
        headers: {
            Authorization: `Bearer ${jwtToken}`,
            "Content-Type": "application/json",
        },
    });

    const data = await response.json();

    const layout = data.layout;
    const held = data.held || [];
    const confirmed = data.confirmed || [];
    const maxCols = Math.max(...layout.map((row) => row.length));
    const prices = data.prices || {};

    const seatPrices = {}; // Lưu giá tiền từng ghế

    layout.forEach((row) => {
        const rowDiv = document.createElement("div");
        rowDiv.className = "seat-row";
        rowDiv.style.gridTemplateColumns = `20px repeat(${maxCols}, 22px) 20px`;

        const rowLetter = row.find(s => s)?.[0] ?? "";

        const labelLeft = document.createElement("div");
        labelLeft.className = "seat-label";
        labelLeft.textContent = rowLetter;
        rowDiv.appendChild(labelLeft);

        for (let i = 0; i < maxCols; i++) {
            const seat = row[i];
            const seatEl = document.createElement("div");

            if (!seat) {
                seatEl.className = "seat empty";
            } else if (seat.includes('+')) {
                seatEl.className = "seat double-seat";
                seatEl.dataset.id = seat;

                const seatNums = seat.split('+').map(s => s.replace(/^[A-Z]/, ""));

                const span1 = document.createElement("span");
                span1.textContent = seatNums[0];
                span1.style.marginRight = "2px";

                const span2 = document.createElement("span");
                span2.textContent = seatNums[1];
                span2.style.marginLeft = "2px";

                seatEl.appendChild(span1);
                seatEl.appendChild(span2);

                if (confirmed.includes(seat)) {
                    seatEl.classList.add("confirmed");
                } else if (held.includes(seat)) {
                    seatEl.classList.add("held");
                } else {
                    seatEl.classList.add("available");
                    seatEl.addEventListener("click", () => {
                        if (!canSelectSeat(seatEl)) return;
                        seatEl.classList.toggle("selected");
                        updateTotal();
                    });
                }

                const rowChar = seat[0];
                const typeInfo = prices[rowChar] || {};
                seatPrices[seat] = typeInfo.price || 0;

            } else {
                const seatNumber = seat.replace(/^[A-Z]/, "");
                seatEl.className = "seat";
                seatEl.textContent = seatNumber;
                seatEl.dataset.id = seat;

                const rowChar = seat[0];
                const typeInfo = prices[rowChar] || {};
                seatEl.title = `${seat} - ${typeInfo.seatType || "Ghế"} - ${typeInfo.price?.toLocaleString()}đ`;

                if (typeInfo.seatType === "VIP") {
                    seatEl.classList.add("vip");
                }

                if (confirmed.includes(seat)) {
                    seatEl.classList.add("confirmed");
                } else if (held.includes(seat)) {
                    seatEl.classList.add("held");
                } else {
                    seatEl.classList.add("available");
                    seatEl.addEventListener("click", () => {
                        if (!canSelectSeat(seatEl)) return;
                        seatEl.classList.toggle("selected");
                        updateTotal();
                    });
                }

                seatPrices[seat] = typeInfo.price || 0;
            }

            rowDiv.appendChild(seatEl);
        }

        const labelRight = document.createElement("div");
        labelRight.className = "seat-label";
        labelRight.textContent = rowLetter;
        rowDiv.appendChild(labelRight);

        seatMatrix.appendChild(rowDiv);
    });
    function canSelectSeat(seatEl) {
        const rowEl = seatEl.closest(".seat-row");
        const seatsInRow = Array.from(rowEl.querySelectorAll(".seat")).filter(s => !s.classList.contains("empty"));

        const index = seatsInRow.indexOf(seatEl);

        // Nếu đang chọn thì tính như "sắp bỏ chọn"
        const selectedCount = document.querySelectorAll(".seat.selected").length;
        const isCurrentlySelected = seatEl.classList.contains("selected");
        const isMaxLimit = selectedCount > 8 && !isCurrentlySelected;
        if (isMaxLimit) {
            alert("Bạn chỉ được chọn tối đa 8 ghế mỗi lần.");
            return false;
        }

        // Nếu click để chọn (chưa có class "selected")
        if (!isCurrentlySelected) {
            // giả lập tình trạng mới nếu chọn ghế này
            seatEl.classList.add("selected");

            const selectedSeats = new Set(
                seatsInRow
                    .filter(s => s.classList.contains("selected") || s.classList.contains("confirmed") || s.classList.contains("held"))
                    .map(s => s.dataset.id)
            );

            let valid = true;
            for (let i = 1; i < seatsInRow.length - 1; i++) {
                const left = seatsInRow[i - 1];
                const middle = seatsInRow[i];
                const right = seatsInRow[i + 1];

                const isLeftSelected = selectedSeats.has(left.dataset.id);
                const isMiddleEmpty = !selectedSeats.has(middle.dataset.id);
                const isRightSelected = selectedSeats.has(right.dataset.id);

                if (isLeftSelected && isRightSelected && isMiddleEmpty && !middle.classList.contains("confirmed") && !middle.classList.contains("held")) {
                    valid = false;
                    break;
                }
            }

            seatEl.classList.remove("selected");

            if (!valid) {
                alert("Không được để trống một ghế giữa các ghế đã chọn hoặc đã bán.");
            }

            return valid;
        }

        return true;
    }

    function updateTotal() {
        const selectedSeats = Array.from(document.querySelectorAll(".seat.selected"))
            .map(seat => seat.dataset.id);

        let total = 0;
        for (const seat of selectedSeats) {
            total += seatPrices[seat] || 0;
        }

        document.getElementById("totalAmount").textContent = total.toLocaleString();

        const totalInput = document.getElementById("totalAmountInput");
        if (totalInput) totalInput.value = total;

        const totalInputVnpay = document.getElementById("totalAmountInputVnpay");
        if (totalInputVnpay) totalInputVnpay.value = total;
    }



    const btnDatVe = document.getElementById("btnDatVe");
    if (btnDatVe) {
        btnDatVe.addEventListener("click", () => {
            const selectedSeats = Array.from(document.querySelectorAll(".seat.selected"))
                .map(seat => seat.dataset.id);

            if (selectedSeats.length === 0) {
                return alert("Vui lòng chọn ít nhất một ghế để tiếp tục.");
            }

            const selectedSeatsInput = document.getElementById("selectedSeatsInput");
            selectedSeatsInput.value = selectedSeats.join(",");
            selectedSeatsInput.closest("form").submit();
        });
    }
});