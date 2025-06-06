using DataAccess.DTO;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Sqlite;

public class SqliteInvoiceRepository : IInvoiceRepository
{
    private readonly SqliteDataAccess _dataAccess;

    public SqliteInvoiceRepository(SqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task<List<InvoiceDTO>> GetInvoices()
    {
        var invoices = new List<InvoiceDTO>();
        using var command = await _dataAccess.CreateCommand();
        command.CommandText = "SELECT id, order_id, status, customer_id FROM invoices";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            invoices.Add(new InvoiceDTO(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetString(2),
                reader.GetGuid(3)
            ));
        }
        return invoices;
    }

    public async Task<InvoiceDTO?> GetInvoice(Guid id)
    {
        using var command = await _dataAccess.CreateCommand();
        command.CommandText = "SELECT id, order_id, status, customer_id FROM invoices WHERE id = @id";
        command.Parameters.AddWithValue("@id", id.ToString().ToLowerInvariant());
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new InvoiceDTO(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetString(2),
                reader.GetGuid(3)
            );
        }
        return null;
    }

    public async Task<InvoiceDTO?> AddInvoice(NewInvoiceDTO invoice)
    {
        try
        {
            using var command = await _dataAccess.CreateCommand();
            command.CommandText = @"
                INSERT INTO invoices (id, order_id, status, customer_id)
                VALUES (@id, @orderId, @status, @customerId)
            ";
            command.Parameters.AddWithValue("@id", invoice.InvoiceId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@orderId", invoice.OrderId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@status", invoice.Status);
            command.Parameters.AddWithValue("@customerId", invoice.CustomerId.ToString().ToLowerInvariant());
            var rows = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"[DEBUG] Invoice insert affected {rows} rows for invoice ID: {invoice.InvoiceId}");
            return new InvoiceDTO(invoice.InvoiceId, invoice.OrderId, invoice.Status, invoice.CustomerId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to insert invoice: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    public async Task UpdateInvoice(InvoiceDTO toUpdate)
    {
        using var command = await _dataAccess.CreateCommand();
        command.CommandText = @"
            UPDATE invoices
            SET order_id = @orderId, status = @status, customer_id = @customerId
            WHERE id = @id
        ";
        command.Parameters.AddWithValue("@id", toUpdate.InvoiceId.ToString().ToLowerInvariant());
        command.Parameters.AddWithValue("@orderId", toUpdate.OrderId.ToString().ToLowerInvariant());
        command.Parameters.AddWithValue("@status", toUpdate.Status);
        command.Parameters.AddWithValue("@customerId", toUpdate.CustomerId.ToString().ToLowerInvariant());
        await command.ExecuteNonQueryAsync();
    }
} 