namespace MovieTicketWebsite.Models
{
    public class HomeIndexViewModel
    {
        public List<Movie> Movies { get; set; }
        public List<ComboUuDai> Combos { get; set; }
        public List<News> News { get; set; }
        public List<Banner> Banners { get; set; } // Thêm dòng này
    }
}
