using Domain.Model;

namespace Infrastructure.Invoice
{
    public interface IInvoice
    {
        byte[] GenerateInvoiceAsync(Email email);
    }
}