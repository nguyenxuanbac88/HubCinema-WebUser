
using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";

        // Hiển thị giỏ hàng
        [HttpGet]
        public IActionResult ViewCart()
        {
            var cart = GetCartFromSession();
            return View("Cart", cart);
        }

        // Tăng số lượng
        [HttpPost]
        public IActionResult IncreaseQuantity(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null) item.Quantity++;

            SaveCartToSession(cart);
            return RedirectToAction("ViewCart");
        }

        // Giảm số lượng
        [HttpPost]
        public IActionResult DecreaseQuantity(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null && item.Quantity > 1)
                item.Quantity--;

            SaveCartToSession(cart);
            return RedirectToAction("ViewCart");
        }

        // Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(i => i.ProductId == productId);

            SaveCartToSession(cart);
            return RedirectToAction("ViewCart");
        }

        // Xóa toàn bộ giỏ hàng
        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("ViewCart");
        }

        // Helper: Lấy giỏ hàng từ session
        private List<CartItem> GetCartFromSession()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(json);
        }

        // Helper: Lưu giỏ hàng vào session
        private void SaveCartToSession(List<CartItem> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, json);
        }
    }
}
