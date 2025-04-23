using Drinkbox.Core.Entities;

namespace Drinkbox.Infrastructure.Services.ExcelImports
{
    public interface IExcelImportService
    {
        /// <summary>
        /// Импортирует продукты из потока Excel-файла
        /// </summary>
        /// <param name="fileStream">Поток данных Excel-файла</param>
        Task ImportProductsAsync(Stream fileStream);
    }
}
