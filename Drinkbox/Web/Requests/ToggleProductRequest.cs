namespace Drinkbox.Web.Requests
{
    /// <summary>
    /// Request для запроса на добавление/удаление товара из корзины.
    /// </summary>
    public record ToggleProductRequest(int productId, bool isSelected);
}
