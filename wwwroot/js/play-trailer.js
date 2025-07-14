let player;

function extractYouTubeVideoId(url) {
    try {
        const parsedUrl = new URL(url);

        // https://www.youtube.com/watch?v=xxxx
        if (parsedUrl.hostname.includes("youtube.com") && parsedUrl.searchParams.has("v")) {
            return parsedUrl.searchParams.get("v");
        }

        // https://youtu.be/xxxx
        if (parsedUrl.hostname.includes("youtu.be")) {
            return parsedUrl.pathname.slice(1);
        }

        // https://www.youtube.com/embed/xxxx
        if (parsedUrl.pathname.startsWith("/embed/")) {
            return parsedUrl.pathname.replace("/embed/", "");
        }
    } catch (e) {
        console.warn("Không thể phân tích URL:", url);
    }

    return null;
}

// Called when YouTube Iframe API is ready
function onYouTubeIframeAPIReady() {
    const playerElement = document.getElementById("player");
    if (!playerElement) return;

    const trailerUrl = playerElement.dataset.src;
    const videoId = extractYouTubeVideoId(trailerUrl);

    if (!videoId) {
        console.warn("Không lấy được videoId từ URL:", trailerUrl);
        return;
    }

    player = new YT.Player("player", {
        videoId: videoId,
        playerVars: {
            autoplay: 0,
            controls: 1,
            modestbranding: 1,
            rel: 0
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const playButton = document.querySelector(".play-button");
    const modalElement = document.getElementById("trailerModal");

    // Load YouTube API script dynamically
    const tag = document.createElement('script');
    tag.src = "https://www.youtube.com/iframe_api";
    document.body.appendChild(tag);

    // Khi user click vào play
    playButton.addEventListener("click", function () {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    });

    // Khi modal hiển thị -> KHÔNG tự động phát video
    modalElement.addEventListener("shown.bs.modal", function () {
        // Không gọi player.playVideo() ở đây
    });

    // Khi modal đóng -> dừng video
    modalElement.addEventListener("hidden.bs.modal", function () {
        if (player && player.stopVideo) {
            player.stopVideo();
        }
    });

    // Khi user nhấn nút play trong iframe, video sẽ phát bình thường
});
