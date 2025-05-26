using DataAccess.DTO;

namespace DataAccess.Repositories;

public interface IInvoiceRepository
{
    Task<List<InvoiceDTO>> GetInvoices();
    Task<InvoiceDTO?> GetInvoice(Guid id);
    Task<InvoiceDTO?> AddInvoice(NewInvoiceDTO i);
    Task UpdateInvoice(InvoiceDTO toUpdate);
}