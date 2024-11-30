﻿namespace Lazza.opal.core.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Status { get; set; }
    }
}
