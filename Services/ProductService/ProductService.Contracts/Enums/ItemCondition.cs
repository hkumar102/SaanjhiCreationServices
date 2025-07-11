namespace ProductService.Contracts.Enums;

/// <summary>
/// Condition rating for inventory items
/// </summary>
public enum ItemCondition
{
    /// <summary>
    /// Brand new, never used
    /// </summary>
    New = 1,

    /// <summary>
    /// Like new, minimal wear
    /// </summary>
    Excellent = 2,

    /// <summary>
    /// Good condition, slight wear
    /// </summary>
    Good = 3,

    /// <summary>
    /// Fair condition, noticeable wear but still rentable
    /// </summary>
    Fair = 4,

    /// <summary>
    /// Poor condition, significant wear
    /// </summary>
    Poor = 5,

    /// <summary>
    /// Damaged, not suitable for rental
    /// </summary>
    Damaged = 6
}
