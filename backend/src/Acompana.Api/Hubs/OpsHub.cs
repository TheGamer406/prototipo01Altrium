/*
 * Archivo:    backend/src/Acompana.Api/Hubs/OpsHub.cs
 * Qué hace:   Hub SignalR /hubs/ops para actualizar pantallas en vivo.
 * Recibe:     Conexiones del frontend con ?branch= (grupo por sede) y grupo 'management' para
 *               gerencia.
 * Procesa:    OnConnectedAsync agrega al grupo; no recibe mensajes del cliente (solo servidor →
 *               cliente).
 * Devuelve:   Eventos: ServiceUpdated, OfferAwarded, AlertRaised, CremationMoved, StockChanged.
 * Se conecta: ← SignalROpsNotifier · → frontend core/realtime/ops-hub.service.ts
 * Reglas:     RF-19 · RF-29 · RNF-06
 * Estado:     Completo
 */

using Microsoft.AspNetCore.SignalR;

namespace Acompana.Api.Hubs;

public sealed class OpsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        var role = http?.Request.Query["role"].ToString();
        var branch = http?.Request.Query["branch"].ToString();
        if (role == "Management") await Groups.AddToGroupAsync(Context.ConnectionId, "management");
        if (!string.IsNullOrEmpty(branch)) await Groups.AddToGroupAsync(Context.ConnectionId, $"branch-{branch}");
        await base.OnConnectedAsync();
    }
}
