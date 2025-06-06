using DataAccess.DTO;
using DataAccess.Repositories;
using System.Threading.Tasks;

namespace Services;

public class InvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<bool> CreateInvoice(NewInvoiceDTO invoice)
    {
        await _invoiceRepository.AddInvoice(invoice);
        return true;
    }
} 