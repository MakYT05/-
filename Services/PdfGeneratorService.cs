using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using InsuranceApp.Models;
using PdfFont = iTextSharp.text.Font;
using PdfRectangle = iTextSharp.text.Rectangle;

namespace InsuranceApp.Services
{
    public static class PdfGeneratorService
    {
        private static readonly string DocumentsFolder = "InsuranceDocuments";
        private static BaseFont _baseFont;

        static PdfGeneratorService()
        {
            if (!Directory.Exists(DocumentsFolder))
            {
                Directory.CreateDirectory(DocumentsFolder);
            }
            
            try
            {
                _baseFont = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", "CP1251", BaseFont.EMBEDDED);
            }
            catch
            {
                try
                {
                    _baseFont = BaseFont.CreateFont("c:/windows/fonts/times.ttf", "CP1251", BaseFont.EMBEDDED);
                }
                catch
                {
                    _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                }
            }
        }

        public static string GenerateInsurancePolicy(Заявка request, Клиент client, Имущество property, Полис policy, Пользователь user)
        {
            try
            {
                if (client == null) throw new Exception("Данные клиента отсутствуют");
                if (property == null) throw new Exception("Данные об имуществе отсутствуют");
                if (policy == null) throw new Exception("Данные о полисе отсутствуют");
                if (user == null) throw new Exception("Данные пользователя отсутствуют");

                string fileName = $"Договор_страхования_{policy.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(DocumentsFolder, fileName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Создаем шрифты на основе загруженного BaseFont
                    PdfFont titleFont = new PdfFont(_baseFont, 18, iTextSharp.text.Font.BOLD);
                    PdfFont headerFont = new PdfFont(_baseFont, 14, iTextSharp.text.Font.BOLD);
                    PdfFont subHeaderFont = new PdfFont(_baseFont, 12, iTextSharp.text.Font.BOLD);
                    PdfFont normalFont = new PdfFont(_baseFont, 11, iTextSharp.text.Font.NORMAL);
                    PdfFont smallFont = new PdfFont(_baseFont, 9, iTextSharp.text.Font.NORMAL);

                    // ЗАГОЛОВОК
                    Paragraph title = new Paragraph("ДОГОВОР СТРАХОВАНИЯ ИМУЩЕСТВА", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 5;
                    document.Add(title);

                    Paragraph number = new Paragraph($"№ {policy.Id}", headerFont);
                    number.Alignment = Element.ALIGN_CENTER;
                    number.SpacingAfter = 15;
                    document.Add(number);

                    Paragraph place = new Paragraph($"г. Севастополь", normalFont);
                    place.Alignment = Element.ALIGN_LEFT;
                    place.SpacingAfter = 5;
                    document.Add(place);

                    Paragraph date = new Paragraph($"{DateTime.Now:dd MMMM yyyy} г.", normalFont);
                    date.Alignment = Element.ALIGN_LEFT;
                    date.SpacingAfter = 20;
                    document.Add(date);

                    // СТОРОНЫ ДОГОВОРА
                    Paragraph insurer = new Paragraph("Общество с ограниченной ответственностью «Севастопольская страховая компания», именуемое в дальнейшем «Страховщик», в лице Генерального директора Иванова Ивана Ивановича, действующего на основании Устава, с одной стороны, и", normalFont);
                    insurer.SpacingAfter = 10;
                    document.Add(insurer);

                    string passportData = string.IsNullOrEmpty(user.Паспорт) ? "паспорт: ________" : $"паспорт: {user.Паспорт}";
                    string addressData = string.IsNullOrEmpty(user.Адрес) ? "зарегистрированный по адресу: ________" : $"зарегистрированный по адресу: {user.Адрес}";
                    
                    Paragraph policyholder = new Paragraph($"гражданин(ка) {user.ФИО}, {passportData}, {addressData}, именуемый(ая) в дальнейшем «Страхователь», с другой стороны, заключили настоящий договор о нижеследующем:", normalFont);
                    policyholder.SpacingAfter = 20;
                    document.Add(policyholder);

                    // 1. ПРЕДМЕТ ДОГОВОРА
                    Paragraph p1Title = new Paragraph("1. ПРЕДМЕТ ДОГОВОРА", subHeaderFont);
                    p1Title.SpacingAfter = 10;
                    document.Add(p1Title);

                    Paragraph p11 = new Paragraph("1.1. Страховщик обязуется за обусловленную настоящим договором плату (страховую премию) при наступлении предусмотренного в договоре события (страхового случая) выплатить Страхователю страховое возмещение в пределах страховой суммы.", normalFont);
                    p11.SpacingAfter = 10;
                    document.Add(p11);

                    Paragraph p12 = new Paragraph("1.2. Объектом страхования является имущество, указанное в п. 1.3 настоящего договора.", normalFont);
                    p12.SpacingAfter = 10;
                    document.Add(p12);

                    Paragraph p13 = new Paragraph("1.3. Застрахованным является следующее имущество:", normalFont);
                    p13.SpacingAfter = 10;
                    document.Add(p13);

                    PdfPTable propertyTable = new PdfPTable(5);
                    propertyTable.WidthPercentage = 100;
                    propertyTable.SetWidths(new float[] { 0.5f, 2f, 2f, 1.5f, 1.5f });

                    propertyTable.AddCell(new PdfPCell(new Phrase("№", subHeaderFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    propertyTable.AddCell(new PdfPCell(new Phrase("Тип имущества", subHeaderFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    propertyTable.AddCell(new PdfPCell(new Phrase("Адрес", subHeaderFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    propertyTable.AddCell(new PdfPCell(new Phrase("Страховая стоимость", subHeaderFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    propertyTable.AddCell(new PdfPCell(new Phrase("Страховая премия", subHeaderFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                    propertyTable.AddCell(new PdfPCell(new Phrase("1", normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    propertyTable.AddCell(new PdfPCell(new Phrase(property.Тип, normalFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    propertyTable.AddCell(new PdfPCell(new Phrase(property.Адрес, normalFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    propertyTable.AddCell(new PdfPCell(new Phrase(property.Стоимость.ToString("N2") + " руб.", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    propertyTable.AddCell(new PdfPCell(new Phrase(policy.Премия.ToString("N2") + " руб.", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                    document.Add(propertyTable);
                    document.Add(new Paragraph(" "));

                    // 2. СТРАХОВЫЕ СЛУЧАИ
                    Paragraph p2Title = new Paragraph("2. СТРАХОВЫЕ СЛУЧАИ", subHeaderFont);
                    p2Title.SpacingAfter = 10;
                    document.Add(p2Title);

                    string[] risks = new string[]
                    {
                        "Пожар, удар молнии, взрыв газа",
                        "Затопление (аварии водопроводных, отопительных и канализационных систем)",
                        "Противоправные действия третьих лиц (кража, грабеж, разбой)",
                        "Стихийные бедствия (буря, ураган, наводнение, град)",
                        "Падение деревьев, летательных аппаратов, наезд транспортных средств",
                        "Повреждение в результате скачков напряжения в электросети"
                    };

                    for (int i = 0; i < risks.Length; i++)
                    {
                        document.Add(new Paragraph($"    2.{i + 1}. {risks[i]}", normalFont));
                    }
                    document.Add(new Paragraph(" "));

                    // 3. СТРАХОВАЯ СУММА И ПРЕМИЯ
                    Paragraph p3Title = new Paragraph("3. СТРАХОВАЯ СУММА И СТРАХОВАЯ ПРЕМИЯ", subHeaderFont);
                    p3Title.SpacingAfter = 10;
                    document.Add(p3Title);

                    document.Add(new Paragraph($"3.1. Страховая сумма по настоящему договору составляет: {property.Стоимость:N2} рублей.", normalFont));
                    document.Add(new Paragraph($"3.2. Страховая премия исчисляется в размере: {policy.Премия:N2} рублей.", normalFont));
                    document.Add(new Paragraph(" "));

                    // 4. СРОК ДЕЙСТВИЯ ДОГОВОРА
                    Paragraph p4Title = new Paragraph("4. СРОК ДЕЙСТВИЯ ДОГОВОРА", subHeaderFont);
                    p4Title.SpacingAfter = 10;
                    document.Add(p4Title);

                    document.Add(new Paragraph($"4.1. Настоящий договор вступает в силу с {policy.ДатаНачала:dd.MM.yyyy} и действует до {policy.ДатаОкончания:dd.MM.yyyy}.", normalFont));
                    document.Add(new Paragraph(" "));

                    // 5. ПРАВА И ОБЯЗАННОСТИ СТОРОН
                    Paragraph p5Title = new Paragraph("5. ПРАВА И ОБЯЗАННОСТИ СТОРОН", subHeaderFont);
                    p5Title.SpacingAfter = 10;
                    document.Add(p5Title);

                    document.Add(new Paragraph("5.1. Страховщик имеет право:", normalFont));
                    document.Add(new Paragraph("    5.1.1. Проверять информацию, сообщаемую Страхователем.", normalFont));
                    document.Add(new Paragraph("    5.1.2. Отказать в выплате страхового возмещения в случаях, предусмотренных законом.", normalFont));
                    document.Add(new Paragraph(" "));

                    document.Add(new Paragraph("5.2. Страховщик обязан:", normalFont));
                    document.Add(new Paragraph("    5.2.1. Ознакомить Страхователя с условиями страхования.", normalFont));
                    document.Add(new Paragraph("    5.2.2. При наступлении страхового случая произвести выплату страхового возмещения.", normalFont));
                    document.Add(new Paragraph(" "));

                    document.Add(new Paragraph("5.3. Страхователь имеет право:", normalFont));
                    document.Add(new Paragraph("    5.3.1. На получение страхового возмещения при наступлении страхового случая.", normalFont));
                    document.Add(new Paragraph("    5.3.2. На досрочное расторжение договора.", normalFont));
                    document.Add(new Paragraph(" "));

                    document.Add(new Paragraph("5.4. Страхователь обязан:", normalFont));
                    document.Add(new Paragraph("    5.4.1. Уплатить страховую премию.", normalFont));
                    document.Add(new Paragraph("    5.4.2. Сообщить Страховщику о наступлении страхового случая.", normalFont));
                    document.Add(new Paragraph("    5.4.3. Предоставить документы, подтверждающие факт страхового случая.", normalFont));
                    document.Add(new Paragraph(" "));

                    // 6. ПОДПИСИ СТОРОН
                    Paragraph p6Title = new Paragraph("6. ПОДПИСИ СТОРОН", subHeaderFont);
                    p6Title.SpacingAfter = 20;
                    document.Add(p6Title);

                    PdfPTable signTable = new PdfPTable(2);
                    signTable.WidthPercentage = 100;
                    signTable.SetWidths(new float[] { 1f, 1f });

                    PdfPCell leftCell = new PdfPCell();
                    leftCell.AddElement(new Paragraph("СТРАХОВЩИК:", normalFont));
                    leftCell.AddElement(new Paragraph("ООО «Севастопольская СК»", normalFont));
                    leftCell.AddElement(new Paragraph("____________________", normalFont));
                    leftCell.AddElement(new Paragraph("Иванов И.И.", normalFont));
                    leftCell.AddElement(new Paragraph("М.П.", smallFont));
                    leftCell.Border = PdfRectangle.NO_BORDER;

                    PdfPCell rightCell = new PdfPCell();
                    rightCell.AddElement(new Paragraph("СТРАХОВАТЕЛЬ:", normalFont));
                    rightCell.AddElement(new Paragraph(user.ФИО, normalFont));
                    rightCell.AddElement(new Paragraph("____________________", normalFont));
                    rightCell.AddElement(new Paragraph("(подпись)", smallFont));
                    rightCell.Border = PdfRectangle.NO_BORDER;

                    signTable.AddCell(leftCell);
                    signTable.AddCell(rightCell);
                    document.Add(signTable);

                    document.Close();
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании PDF: {ex.Message}");
            }
        }
    }
}