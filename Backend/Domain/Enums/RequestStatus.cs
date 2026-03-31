namespace Domain.Enums;

public enum RequestStatus
{
    /// <summary>Đã tiếp nhận (Tổng chi phí <= 100 triệu)</summary>
    Received,

    /// <summary>Chờ duyệt quản lý (Tổng chi phí > 100 triệu)</summary>
    PendingApproval,

    /// <summary>Đã phê duyệt</summary>
    Approved
}
