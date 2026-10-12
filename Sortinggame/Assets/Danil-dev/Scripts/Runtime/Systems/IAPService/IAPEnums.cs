namespace D_Dev.IAPService
{
    public enum IAPProductType
    {
        Consumable = 0,
        NonConsumable = 1
    }

    public enum IAPPurchaseResult
    {
        Success = 0,
        Cancelled = 1,
        Failed = 2,
        NotInitialized = 3,
        UnknownProduct = 4,
        AlreadyPurchasing = 5
    }
}
