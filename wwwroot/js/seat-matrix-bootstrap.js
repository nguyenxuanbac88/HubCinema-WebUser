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

        const selectedCount = document.querySelectorAll(".seat.selected").length;
        const isCurrentlySelected = seatEl.classList.contains("selected");

        // ✅ Luật 4: Không vượt quá 8 ghế
        if (selectedCount >= 8 && !isCurrentlySelected) {
            showToast("Bạn chỉ được chọn tối đa 8 ghế mỗi lần.");
            return false;
        }

        // ✅ Nếu đang chọn mới kiểm tra
        if (!isCurrentlySelected) {
            seatEl.classList.add("selected");

            const selectedSeats = new Set(
                seatsInRow
                    .filter(s => s.classList.contains("selected") || s.classList.contains("confirmed") || s.classList.contains("held"))
                    .map(s => s.dataset.id)
            );

            let valid = true;
            let reason = "";

            // ✅ Luật 1: Không để trống 1 ghế giữa
            for (let i = 1; i < seatsInRow.length - 1; i++) {
                const left = seatsInRow[i - 1];
                const middle = seatsInRow[i];
                const right = seatsInRow[i + 1];

                const isLeftSelected = selectedSeats.has(left.dataset.id);
                const isMiddleEmpty = !selectedSeats.has(middle.dataset.id);
                const isRightSelected = selectedSeats.has(right.dataset.id);

                if (
                    isLeftSelected && isRightSelected && isMiddleEmpty &&
                    !middle.classList.contains("confirmed") &&
                    !middle.classList.contains("held")
                ) {
                    valid = false;
                    reason = "Không được để trống 1 ghế giữa các ghế đã chọn hoặc đã bán.";
                    break;
                }
            }

            // ✅ Luật 2: Không để trống 2 ghế liền giữa
            if (valid) {
                for (let i = 1; i < seatsInRow.length - 2; i++) {
                    const s1 = seatsInRow[i];
                    const s2 = seatsInRow[i + 1];
                    const left = seatsInRow[i - 1];
                    const right = seatsInRow[i + 2];

                    if (
                        !selectedSeats.has(s1.dataset.id) &&
                        !selectedSeats.has(s2.dataset.id) &&
                        selectedSeats.has(left.dataset.id) &&
                        selectedSeats.has(right.dataset.id) &&
                        !s1.classList.contains("confirmed") &&
                        !s2.classList.contains("confirmed")
                    ) {
                        valid = false;
                        reason = "Không được để trống 2 ghế liền nhau giữa các ghế đã chọn.";
                        break;
                    }
                }
            }

            // ✅ Luật 3: Không chọn cách xa nhau
            if (valid) {
                const selectedIndexes = seatsInRow
                    .map((s, i) => selectedSeats.has(s.dataset.id) ? i : -1)
                    .filter(i => i !== -1);

                for (let i = 1; i < selectedIndexes.length; i++) {
                    if (selectedIndexes[i] - selectedIndexes[i - 1] > 1) {
                        valid = false;
                        reason = "Các ghế được chọn phải nằm liền kề, không được cách xa.";
                        break;
                    }
                }
            }

            // ✅ Luật 5: Không trộn ghế đôi và thường
            if (valid) {
                const selectedSeatEls = Array.from(document.querySelectorAll(".seat.selected"));
                const hasDouble = selectedSeatEls.some(s => s.classList.contains("double-seat"));
                const hasSingle = selectedSeatEls.some(s => !s.classList.contains("double-seat"));

                if (hasDouble && hasSingle) {
                    valid = false;
                    reason = "Không được chọn đồng thời ghế đơn và ghế đôi.";
                }
            }
            // ✅ Luật 7: Ghế đang held (đã được xử lý từ trước bằng class 'held') — bỏ qua

            seatEl.classList.remove("selected");

            if (!valid) {
                showToast(reason);
                return false;
            }
        }

        return true;
    }
    function showToast(message) {
        // Bạn có thể thay bằng SweetAlert2, Toastify, hoặc tự viết modal
        alert("Thông báo:\n" + message);
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