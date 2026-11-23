/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/MockControlClient.cs
 * Qué hace:   Implementa IMockControlClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /_control/*; serializa JSON; reintentos según ResiliencePolicies.
 * Devuelve:   El resultado definido en IMockControlClient.
 * Se conecta: ← Application/Integrations/IMockControlClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Pendiente
 */
