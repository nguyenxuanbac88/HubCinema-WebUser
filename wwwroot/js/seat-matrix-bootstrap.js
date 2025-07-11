document.addEventListener("DOMContentLoaded", async () => {
    const seatMatrix = document.getElementById("seat-matrix");

    // 👇 Lấy token bằng fetch, không để lộ trực tiếp trong HTML
    const tokenRes = await fetch("/Login/GetJwt");
    if (!tokenRes.ok) return alert("Không lấy được token");

    const { token: jwtToken } = await tokenRes.json();


    // 🟨 Fetch dữ liệu với JWT trong Authorization header
    const apiUrl =
        "http://api.dvxuanbac.com:2030/api/Seat/get-layout-price/1751954725262/3";
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

    layout.forEach((row) => {
        const rowDiv = document.createElement("div");
        rowDiv.className = "seat-row";
        rowDiv.style.gridTemplateColumns = `auto repeat(${maxCols}, 36px) auto`; // thu nhỏ ghế tại đây nếu cần

        const rowLetter = row.find((seat) => seat)?.[0] ?? "";

        // Label trái
        const labelLeft = document.createElement("div");
        labelLeft.className = "seat-label";
        labelLeft.textContent = rowLetter;
        rowDiv.appendChild(labelLeft);

        // Ghế
        for (let i = 0; i < maxCols; i++) {
            const seat = row[i];
            const seatEl = document.createElement("div");
            seatEl.classList.add("seat");

            if (!seat) {
                seatEl.classList.add("empty");
            } else {
                const seatNumber = seat.slice(1); // Bỏ ký tự A/B/C..., chỉ lấy số
                seatEl.textContent = seatNumber;
                seatEl.dataset.id = seat;

                if (confirmed.includes(seat)) {
                    seatEl.classList.add("confirmed");
                } else if (held.includes(seat)) {
                    seatEl.classList.add("held");
                } else {
                    seatEl.classList.add("available");

                    // Chọn ghế
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
