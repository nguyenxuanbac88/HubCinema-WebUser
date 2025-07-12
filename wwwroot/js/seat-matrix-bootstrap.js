document.addEventListener("DOMContentLoaded", async () => {
    const seatMatrix = document.getElementById("seat-matrix");
    const idSuatChieu = seatMatrix.getAttribute('data-id-suat-chieu');

    // 👇 Lấy token bằng fetch, không để lộ trực tiếp trong HTML
    const tokenRes = await fetch("/Login/GetJwt");
    if (!tokenRes.ok) return alert("Không lấy được token");
    

    const { token: jwtToken } = await tokenRes.json();
    console.log("Token:", jwtToken);

    // 🟨 Fetch dữ liệu với JWT trong Authorization header
    const apiUrl =
        `http://api.dvxuanbac.com:2030/api/Seat/get-layout-price/${idSuatChieu}`;
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

    layout.forEach((row) => {
        const rowDiv = document.createElement("div");
        rowDiv.className = "seat-row";
        rowDiv.style.gridTemplateColumns = `24px repeat(${maxCols}, 1fr) 24px`;

        // Lấy tên hàng (bỏ qua null)
        const rowLetter = row.find(s => s)?.[0] ?? "";

        // Label trái
        const labelLeft = document.createElement("div");
        labelLeft.className = "seat-label";
        labelLeft.textContent = rowLetter;
        rowDiv.appendChild(labelLeft);

        // Duyệt từng cột
        for (let i = 0; i < maxCols; i++) {
            const seat = row[i];
            const seatEl = document.createElement("div");

            if (!seat) {
                seatEl.className = "seat empty";
            } else if (seat.includes('+')) {
                // Ghế đôi
                seatEl.className = "seat double-seat";
                seatEl.textContent = seat.replace('+', ' + ');
                seatEl.dataset.id = seat;
                seatEl.classList.add("available");

                // Không chia đôi ghế nữa
                if (confirmed.includes(seat)) {
                    seatEl.classList.add("confirmed");
                } else if (held.includes(seat)) {
                    seatEl.classList.add("held");
                } else {
                    seatEl.addEventListener("click", () => {
                        seatEl.classList.toggle("selected");
                    });
                }
            } else {
                const seatNumber = seat.slice(1);
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
                        seatEl.classList.toggle("selected");
                    });
                }
            }

            rowDiv.appendChild(seatEl);
        }

        // Label phải
        const labelRight = document.createElement("div");
        labelRight.className = "seat-label";
        labelRight.textContent = rowLetter;
        rowDiv.appendChild(labelRight);


        seatMatrix.appendChild(rowDiv);
    });
});
