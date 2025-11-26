using System;
using System.Collections.Generic;
using System.Text;

namespace CinePass.Domain;

// Vai trò người dùng
public enum UserRole
{
    Customer,
    Staff,
    Admin
}

// Loại ghế
public enum SeatType
{
    Regular,
    VIP
}

// Trạng thái đặt vé
public enum BookingStatus
{
    Pending,    // Chưa thanh toán
    Paid,       // Đã thanh toán
    Cancelled,  // Hủy
    CheckedIn   // Đã check-in tại rạp
}

// Phương thức thanh toán
public enum PaymentMethod
{
    VnPay,
    MoMo,
    Cash
}

// Trạng thái thanh toán
public enum PaymentStatus
{
    Pending,    // Chưa thanh toán
    Success,    // Thanh toán thành công
    Failed      // Thanh toán thất bại
}