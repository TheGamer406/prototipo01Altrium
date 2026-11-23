/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/InsurerClient.cs
 * Qué hace:   Implementa IInsurerClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /aseguradoras/notices; serializa JSON; reintentos según ResiliencePolicies.
 * Devuelve:   El resultado definido en IInsurerClient.
 * Se conecta: ← Application/Integrations/IInsurerClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Pendiente
 */
