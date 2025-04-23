using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Drinkbox.Core.Entities;
using Drinkbox.Infrastructure.Data;

namespace Drinkbox.Infrastructure.Services.ExcelImports
{
    /// <summary>
    /// Сервис для импорта продуктов из Excel-файлов
    /// </summary>
    public class ExcelImportService : IExcelImportService
    {
        private readonly VendomatContext _context;
        private readonly ILogger<ExcelImportService> _logger;

        public ExcelImportService(VendomatContext context, ILogger<ExcelImportService> logger)
        {
            _context = context;
            _logger = logger;
        }
       
        public async Task ImportProductsAsync(Stream fileStream)
        {
            var products = new List<Product>();
            var tempFilePath = Path.GetTempFileName(); // Создает временный файл для обработки

            try
            {
                await using (var tempFile = File.Create(tempFilePath)) // Копирует поток во временный файл
                {
                    await fileStream.CopyToAsync(tempFile);
                }

                using (var document = SpreadsheetDocument.Open(tempFilePath, false))
                {
                    var workbookPart = document.WorkbookPart;
                    var worksheetPart = workbookPart?.WorksheetParts.First();
                    var sheetData = worksheetPart?.Worksheet.Elements<SheetData>().First();

                    if (sheetData == null)
                    {
                        throw new InvalidOperationException("Не удалось прочитать данные из файла");
                    }

                    var rowIndex = 0;
                    foreach (var row in sheetData.Elements<Row>())
                    {
                        rowIndex++;
                        if (rowIndex == 1) continue;

                        var cells = row.Elements<Cell>().ToList();
                        if (IsEmptyRow(cells, workbookPart))
                        {
                            _logger.LogWarning($"Пустая строка {rowIndex} пропущена");
                            continue;
                        }

                        products.Add(new Product
                        {
                            ProductName = GetCellValue(cells[0], workbookPart),
                            Price = int.Parse(GetCellValue(cells[1], workbookPart)),
                            Quantity = int.Parse(GetCellValue(cells[2], workbookPart)),
                            ImageUrl = GetCellValue(cells[3], workbookPart),
                            BrandId = int.Parse(GetCellValue(cells[4], workbookPart)),
                        });
                    }
                }
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Импортировано {products.Count} продуктов");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка обработки строк");
                throw new InvalidOperationException($"Ошибка в строк: {ex.Message}");
            }
            finally
            {
                // Удаляет временный файл в любом случае
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        /// <summary>
        /// Получает текстовое значение ячейки Excel
        /// </summary>
        /// <param name="cell">Ячейка Excel</param>
        /// <param name="workbookPart">Часть книги Excel</param>
        /// <returns>Текстовое значение ячейки</returns>
        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            if (cell?.CellValue == null) return string.Empty;

            var value = cell.CellValue.Text;

            // Обработка shared strings (общих строк)
            if (cell.DataType?.Value == CellValues.SharedString)
            {
                var stringTable = workbookPart?.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (stringTable != null)
                {
                    value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }

            return value.Trim();
        }

        /// <summary>
        /// Проверяет, является ли строка пустой
        /// </summary>
        /// <param name="cells">Список ячеек строки</param>
        /// <param name="workbookPart">Часть книги Excel</param>
        /// <returns>True, если все ячейки строки пустые</returns>
        private bool IsEmptyRow(List<Cell> cells, WorkbookPart workbookPart)
        {
            foreach (var cell in cells)
            {
                var value = GetCellValue(cell, workbookPart);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return false; 
                }
            }
            return true; 
        }
    }
}
