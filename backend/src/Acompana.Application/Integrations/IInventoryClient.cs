/*
 * Archivo:    backend/src/Acompana.Application/Integrations/IInventoryClient.cs
 * Qué hace:   Cliente del sistema de inventario actual y de la bodega central.
 * Recibe:     —
 * Procesa:    Interfaz; la implementación vive en Infrastructure/Integrations.
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Integrations (HttpClient al servicio simulado)
 * Reglas:     RF-26 · RF-28
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<int> ReportMovementAsync(long branchId, string sku, int delta, CancellationToken ct) ·
 *       Task<string> RequestRestockAsync(long branchId, string sku, int qty, CancellationToken ct)
 *     Recibe:     sede, SKU del cofre, cantidad (negativa = salida)
 *     Procesa:    POST /inventario/movements y POST /inventario/restock con reintentos
 *     Devuelve:   saldo informado por el sistema externo · número de solicitud de bodega (p. ej.
 *                   'BOD-000045')
 */
