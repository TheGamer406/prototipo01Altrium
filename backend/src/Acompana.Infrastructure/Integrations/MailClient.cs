/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/MailClient.cs
 * Qué hace:   Implementa IMailClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /mail/send + insert en outbound_messages; serializa JSON; reintentos según
 *               ResiliencePolicies.
 * Devuelve:   El resultado definido en IMailClient.
 * Se conecta: ← Application/Integrations/IMailClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Pendiente
 */
