namespace MovieTicketWebsite.Models
{
    //dùng để deserialize $"{_baseApiUrl}/Schedule/GetMovieIdsByCinema?maRap={cinema.MaRap}" trong BookingController
    public class IntArrayResponse
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public List<int> Data { get; set; } = new();
    }
}
