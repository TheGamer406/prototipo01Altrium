/*
 * Archivo:    backend/tests/Acompana.Tests/ChapelOverlapTests.cs
 * Qué hace:   Prueba xUnit: Capilla sin traslape.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    dos reservas que se cruzan en la misma capilla → la segunda falla con 409 y mensaje con
 *               la reserva que choca
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-20
 * Estado:     Pendiente
 */
