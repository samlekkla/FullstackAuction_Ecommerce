using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.Data.Enums
{
    public enum AuctionStatus
    {
        Draft = 0,
        Active = 1,
        Completed = 2,
        Cancelled = 3,
        Expired = 4
    }

    public enum BidStatus
    {
        Active = 0,
        Winning = 1,
        Outbid = 2,
        Cancelled = 3
    }
}
