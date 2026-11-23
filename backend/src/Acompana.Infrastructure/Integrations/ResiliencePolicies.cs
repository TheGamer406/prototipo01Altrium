/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/ResiliencePolicies.cs
 * Qué hace:   Políticas Polly compartidas por los clientes HTTP.
 * Recibe:     —
 * Procesa:    3 reintentos con espera exponencial (0,5 s, 1 s, 2 s) ante 5xx, 408 y errores de red;
 *               timeout 5 s por intento.
 * Devuelve:   IAsyncPolicy<HttpResponseMessage>.
 * Se conecta: ← DependencyInjection (AddPolicyHandler)
 * Reglas:     RNF-15
 * Estado:     Pendiente
 */
