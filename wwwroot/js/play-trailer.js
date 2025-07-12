let player;

function onYouTubeIframeAPIReady() {
    player = new YT.Player('player', {
        videoId: 'EQ9CGrgIq9M',
        playerVars: {
            autoplay: 0, // Không tự động phát
            controls: 1,
            rel: 0,
            modestbranding: 1
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
