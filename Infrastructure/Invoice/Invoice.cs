using Domain.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace Infrastructure.Invoice
{
    public class Invoice : IInvoice
    {
        public byte[] GenerateInvoiceAsync(Email email)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(column =>
                            {
                                column.Item().Text("Travel & Accommodation Booking Platform").Bold().FontSize(16);
                                column.Item().Text("Payment Receipt").SemiBold().FontSize(14);
                            });
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text("Customer Details:").Bold();
                            column.Item().Text($"Name: {email.Name}");
                            column.Item().Text($"Email: {email.ToEmail}");

                            column.Item().PaddingTop(10).Text("Payment Details:").Bold();
                            column.Item().Text($"Transaction ID: {email.BookingId}");
                            column.Item().Text($"Amount: {email.Amount:C}");
                            column.Item().Text($"Payment Method: {email.PaymentMethod}");
                            column.Item().Text($"Transaction Date: {DateTime.UtcNow:dd/MM/yyyy HH:mm}");
                            column.Item().Text($"Status: {email.PaymentStatus}");

                            column.Item().PaddingTop(10).Text("Booking Details:").Bold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Room Number
                                    columns.RelativeColumn(); // Room Type
                                    columns.RelativeColumn(); // Check-In Date
                                    columns.RelativeColumn(); // Check-Out Date
                                    columns.RelativeColumn(); // Price
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Room Number").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Room Type").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Check-In Date").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Check-Out Date").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Price").Bold();
                                });



                                table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text(email.Booking.Room.Id);
                                table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text(email.Booking.Room.RoomType);
                                table.Cell().Background(Colors.Grey.Lighten4).Padding(5)
                                    .Text(email.Booking.CheckInDate.ToString("dd/MM/yyyy"));
                                table.Cell().Background(Colors.Grey.Lighten4).Padding(5)
                                    .Text(email.Booking.CheckOutDate.ToString("dd/MM/yyyy"));
                                table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text($"{email.Booking.Price:C}");

                            });

                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for choosing our platform! Contact us at support@tabp.com for any queries.")
                        .FontSize(10);
                });
            }).GeneratePdf();

            return pdfBytes;
        }
    }
}
