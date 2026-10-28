namespace ParkEasePro.Models;

public enum OrderStatus
{
    Placed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Refunded = 3,
    Failed = 4
}

public enum PaymentMethod
{
    UPI = 1,
    CreditDebitCard = 2,
    NetBanking = 3,
    CashOnDelivery = 4
}
