/*
 * Archivo:    backend/src/Acompana.Application/Common/IntegrationUnavailableException.cs
 * Qué hace:   Excepción cuando un sistema externo simulado no responde tras los reintentos.
 * Recibe:     Mensaje en español.
 * Procesa:    Solo transporta el error.
 * Devuelve:   La atrapa ErrorHandlingMiddleware y responde 503.
 * Se conecta: ← clientes de Infrastructure/Integrations · → ErrorHandlingMiddleware
 * Reglas:     RNF-15
 * Estado:     Completo
 */
namespace Acompana.Application.Common;

public class IntegrationUnavailableException : Exception
{
    public IntegrationUnavailableException(string message, Exception? inner = null) : base(message, inner) { }
}
