/*
 * Archivo:    backend/src/Acompana.Application/DependencyInjection.cs
 * Qué hace:   Registra en el contenedor todos los handlers, queries y servicios de Application.
 * Recibe:     IServiceCollection desde Program.cs.
 * Procesa:    AddScoped de cada handler/query; AddSingleton de InvoiceCalculator.
 * Devuelve:   La misma IServiceCollection (encadenable).
 * Se conecta: ← Api/Program.cs (AddApplication())
 * Estado:     Pendiente
 */
