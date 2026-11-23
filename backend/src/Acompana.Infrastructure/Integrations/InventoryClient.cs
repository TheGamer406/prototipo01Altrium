/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/InventoryClient.cs
 * Qué hace:   Implementa IInventoryClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /inventario/movements, /inventario/restock; serializa JSON; reintentos según
 *               ResiliencePolicies.
 * Devuelve:   El resultado definido en IInventoryClient.
 * Se conecta: ← Application/Integrations/IInventoryClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Pendiente
 */
