let player;

function onYouTubeIframeAPIReady() {
    player = new YT.Player('player', {
        videoId: 'EQ9CGrgIq9M',
        playerVars: {
            autoplay: 1,
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

    // Khi modal hiển thị -> phát video
    modalElement.addEventListener("shown.bs.modal", function () {
        if (player && player.playVideo) {
            player.playVideo();
        }
    });

    // Khi modal đóng -> dừng video
    modalElement.addEventListener("hidden.bs.modal", function () {
        if (player && player.stopVideo) {
            player.stopVideo();
        }
    });
});
