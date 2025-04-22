namespace Drinkbox.Web.Requests
{
    /// <summary>
    /// Request для запроса на обновление количества товара.
    /// </summary>
    public record UpdateQuantityRequest(int productId, int quantity);
}
