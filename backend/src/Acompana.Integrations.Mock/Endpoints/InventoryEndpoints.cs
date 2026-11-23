/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/InventoryEndpoints.cs
 * Qué hace:   Simula el sistema de inventario actual y la bodega central.
 * Recibe:     POST /inventario/movements { branchId, sku, delta } · POST /inventario/restock {
 *               branchId, sku, qty }.
 * Procesa:    Lleva un saldo ficticio por sede/SKU; numera solicitudes BOD-000001.
 * Devuelve:   200 { balance } · 201 { requestNumber }.
 * Se conecta: ← InventoryClient
 * Reglas:     RF-26 · RF-28
 * Estado:     Completo
 */

namespace Acompana.Integrations.Mock.Endpoints;

public record MovementBody(long BranchId, string Sku, int Delta);
public record RestockBody(long BranchId, string Sku, int Qty);

public static class InventoryEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/inventario/movements", async (MovementBody b, MockState state) =>
        {
            await Latency.Wait();
            var balance = state.Balances.AddOrUpdate($"{b.BranchId}:{b.Sku}", 5 + b.Delta, (_, cur) => Math.Max(0, cur + b.Delta));
            return Results.Ok(new { balance });
        });
        app.MapPost("/inventario/restock", async (RestockBody b, MockState state) =>
        {
            await Latency.Wait();
            return Results.Created("/inventario/restock", new { requestNumber = state.NextRestockNumber() });
        });
    }
}
