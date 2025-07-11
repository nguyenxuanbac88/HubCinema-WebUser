﻿namespace MovieTicketWebsite.Models
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
