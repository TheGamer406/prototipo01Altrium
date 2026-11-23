/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/CoreAccountingClient.cs
 * Qué hace:   Implementa ICoreAccountingClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /core-contable/invoices; serializa JSON; reintentos según ResiliencePolicies.
 * Devuelve:   El resultado definido en ICoreAccountingClient.
 * Se conecta: ← Application/Integrations/ICoreAccountingClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Pendiente
 */
