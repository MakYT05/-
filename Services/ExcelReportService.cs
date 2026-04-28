using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using InsuranceApp.Models;

namespace InsuranceApp.Services
{
    public static class ExcelReportService
    {
        public static string CreateReport(string filePath)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var summarySheet = package.Workbook.Worksheets.Add("Общая статистика");
                    FillSummarySheet(summarySheet);

                    var policiesSheet = package.Workbook.Worksheets.Add("Полисы");
                    FillPoliciesSheet(policiesSheet);

                    var clientsSheet = package.Workbook.Worksheets.Add("Клиенты");
                    FillClientsSheet(clientsSheet);

                    var propertiesSheet = package.Workbook.Worksheets.Add("Имущество");
                    FillPropertiesSheet(propertiesSheet);

                    var requestsSheet = package.Workbook.Worksheets.Add("Заявки");
                    FillRequestsSheet(requestsSheet);

                    var bookingsSheet = package.Workbook.Worksheets.Add("Бронирования");
                    FillBookingsSheet(bookingsSheet);

                    var file = new FileInfo(filePath);
                    package.SaveAs(file);

                    return filePath;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании отчета: {ex.Message}");
            }
        }

        private static void FillSummarySheet(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "ОТЧЕТ ПО СТРАХОВОЙ КОМПАНИИ";
            sheet.Cells["A1:C1"].Merge = true;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells["A3"].Value = "Дата формирования отчета:";
            sheet.Cells["B3"].Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            sheet.Cells["A3"].Style.Font.Bold = true;

            var полисы = DatabaseService.GetPolicies();
            var клиенты = DatabaseService.GetClients();
            var имущество = DatabaseService.GetProperties();
            var заявки = DatabaseService.ПолучитьЗаявки();

            int row = 5;
            
            sheet.Cells[$"A{row}"].Value = "ОБЩАЯ СТАТИСТИКА:";
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            sheet.Cells[$"A{row}"].Style.Font.Size = 12;
            row += 2;

            sheet.Cells[$"A{row}"].Value = "Всего полисов:";
            sheet.Cells[$"B{row}"].Value = полисы.Count;
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            row++;

            int activePolicies = полисы.Count(p => p.ДатаОкончания > DateTime.Now);
            sheet.Cells[$"A{row}"].Value = "Активных полисов:";
            sheet.Cells[$"B{row}"].Value = activePolicies;
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            row++;

            int expiredPolicies = полисы.Count(p => p.ДатаОкончания <= DateTime.Now);
            sheet.Cells[$"A{row}"].Value = "Истекших полисов:";
            sheet.Cells[$"B{row}"].Value = expiredPolicies;
            row += 2;

            sheet.Cells[$"A{row}"].Value = "ФИНАНСОВАЯ СТАТИСТИКА:";
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            sheet.Cells[$"A{row}"].Style.Font.Size = 12;
            row += 2;

            decimal totalPremium = полисы.Sum(p => p.Премия);
            sheet.Cells[$"A{row}"].Value = "Общая сумма страховых премий:";
            sheet.Cells[$"B{row}"].Value = totalPremium;
            sheet.Cells[$"B{row}"].Style.Numberformat.Format = "#,##0.00 ₽";
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            row++;

            decimal avgPremium = полисы.Count > 0 ? полисы.Average(p => p.Премия) : 0;
            sheet.Cells[$"A{row}"].Value = "Средняя премия:";
            sheet.Cells[$"B{row}"].Value = avgPremium;
            sheet.Cells[$"B{row}"].Style.Numberformat.Format = "#,##0.00 ₽";
            row++;

            decimal totalInsured = полисы.Sum(p => 
            {
                var prop = имущество.FirstOrDefault(i => i.Id == p.ИмуществоId);
                return prop?.Стоимость ?? 0;
            });
            sheet.Cells[$"A{row}"].Value = "Общая страховая сумма:";
            sheet.Cells[$"B{row}"].Value = totalInsured;
            sheet.Cells[$"B{row}"].Style.Numberformat.Format = "#,##0.00 ₽";
            row += 2;

            sheet.Cells[$"A{row}"].Value = "СТАТИСТИКА ПО ЗАЯВКАМ:";
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            sheet.Cells[$"A{row}"].Style.Font.Size = 12;
            row += 2;

            sheet.Cells[$"A{row}"].Value = "Всего заявок:";
            sheet.Cells[$"B{row}"].Value = заявки.Count;
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            row++;

            sheet.Cells[$"A{row}"].Value = "Средняя стоимость в заявках:";
            decimal avgRequestCost = заявки.Count > 0 ? заявки.Average(r => r.Стоимость) : 0;
            sheet.Cells[$"B{row}"].Value = avgRequestCost;
            sheet.Cells[$"B{row}"].Style.Numberformat.Format = "#,##0.00 ₽";
            row += 2;

            sheet.Cells[$"A{row}"].Value = "СТАТИСТИКА ПО ТИПАМ ИМУЩЕСТВА:";
            sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            sheet.Cells[$"A{row}"].Style.Font.Size = 12;
            row += 2;

            var propertyTypes = имущество.GroupBy(i => i.Тип)
                                         .Select(g => new { Тип = g.Key, Количество = g.Count(), ОбщаяСтоимость = g.Sum(i => i.Стоимость) })
                                         .ToList();

            sheet.Cells[$"A{row}"].Value = "Тип имущества";
            sheet.Cells[$"B{row}"].Value = "Количество";
            sheet.Cells[$"C{row}"].Value = "Общая стоимость";
            sheet.Cells[$"A{row}:C{row}"].Style.Font.Bold = true;
            sheet.Cells[$"A{row}:C{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[$"A{row}:C{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            row++;

            foreach (var type in propertyTypes)
            {
                sheet.Cells[$"A{row}"].Value = type.Тип;
                sheet.Cells[$"B{row}"].Value = type.Количество;
                sheet.Cells[$"C{row}"].Value = type.ОбщаяСтоимость;
                sheet.Cells[$"C{row}"].Style.Numberformat.Format = "#,##0.00 ₽";
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private static void FillPoliciesSheet(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "СПИСОК ВСЕХ ПОЛИСОВ";
            sheet.Cells["A1:H1"].Merge = true;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var полисы = DatabaseService.GetPolicies();
            var клиенты = DatabaseService.GetClients();
            var имущество = DatabaseService.GetProperties();

            string[] headers = { "ID", "Клиент", "Телефон", "Объект", "Адрес", "Стоимость", "Премия", "Дата начала", "Дата окончания", "Статус" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 4;
            foreach (var p in полисы)
            {
                var client = клиенты.FirstOrDefault(c => c.Id == p.КлиентId);
                var prop = имущество.FirstOrDefault(i => i.Id == p.ИмуществоId);

                sheet.Cells[row, 1].Value = p.Id;
                sheet.Cells[row, 2].Value = client?.ФИО ?? "Неизвестно";
                sheet.Cells[row, 3].Value = client?.Телефон ?? "";
                sheet.Cells[row, 4].Value = prop?.Тип ?? "";
                sheet.Cells[row, 5].Value = prop?.Адрес ?? "";
                sheet.Cells[row, 6].Value = prop?.Стоимость ?? 0;
                sheet.Cells[row, 6].Style.Numberformat.Format = "#,##0.00 ₽";
                sheet.Cells[row, 7].Value = p.Премия;
                sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00 ₽";
                sheet.Cells[row, 8].Value = p.ДатаНачала.ToString("dd.MM.yyyy");
                sheet.Cells[row, 9].Value = p.ДатаОкончания.ToString("dd.MM.yyyy");
                
                string status = p.ДатаОкончания > DateTime.Now ? "Активен" : "Истек";
                sheet.Cells[row, 10].Value = status;
                
                if (status == "Активен")
                    sheet.Cells[row, 10].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                else
                    sheet.Cells[row, 10].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private static void FillClientsSheet(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "СПИСОК КЛИЕНТОВ";
            sheet.Cells["A1:C1"].Merge = true;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var клиенты = DatabaseService.GetClients();

            string[] headers = { "ID", "ФИО", "Телефон" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 4;
            foreach (var c in клиенты)
            {
                sheet.Cells[row, 1].Value = c.Id;
                sheet.Cells[row, 2].Value = c.ФИО;
                sheet.Cells[row, 3].Value = c.Телефон;
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private static void FillPropertiesSheet(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "СПИСОК ИМУЩЕСТВА";
            sheet.Cells["A1:D1"].Merge = true;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var имущество = DatabaseService.GetProperties();

            string[] headers = { "ID", "Тип", "Стоимость", "Адрес" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 4;
            foreach (var p in имущество)
            {
                sheet.Cells[row, 1].Value = p.Id;
                sheet.Cells[row, 2].Value = p.Тип;
                sheet.Cells[row, 3].Value = p.Стоимость;
                sheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00 ₽";
                sheet.Cells[row, 4].Value = p.Адрес;
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private static void FillRequestsSheet(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "СПИСОК ЗАЯВОК";
            sheet.Cells["A1:H1"].Merge = true;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var заявки = DatabaseService.ПолучитьЗаявки();

            string[] headers = { "ID", "ФИО", "Телефон", "Тип", "Стоимость", "Адрес", "Премия", "Срок" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 4;
            foreach (var r in заявки)
            {
                sheet.Cells[row, 1].Value = r.Id;
                sheet.Cells[row, 2].Value = r.ФИО;
                sheet.Cells[row, 3].Value = r.Телефон;
                sheet.Cells[row, 4].Value = r.ТипИмущества;
                sheet.Cells[row, 5].Value = r.Стоимость;
                sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00 ₽";
                sheet.Cells[row, 6].Value = r.Адрес;
                sheet.Cells[row, 7].Value = r.Премия;
                sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00 ₽";
                sheet.Cells[row, 8].Value = r.Срок;
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private static void FillBookingsSheet(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "СПИСОК БРОНИРОВАНИЙ";
            sheet.Cells["A1:F1"].Merge = true;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var бронирования = DatabaseService.ПолучитьБронирования();

            string[] headers = { "ID", "ФИО", "Телефон", "Дата", "Время", "Статус" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 4;
            foreach (var b in бронирования)
            {
                sheet.Cells[row, 1].Value = b.Id;
                sheet.Cells[row, 2].Value = b.ФИО;
                sheet.Cells[row, 3].Value = b.Телефон;
                sheet.Cells[row, 4].Value = b.Дата;
                sheet.Cells[row, 5].Value = b.Время;
                sheet.Cells[row, 6].Value = b.Статус;
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }
    }
}