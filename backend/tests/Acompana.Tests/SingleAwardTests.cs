/*
 * Archivo:    backend/tests/Acompana.Tests/SingleAwardTests.cs
 * Qué hace:   Prueba xUnit: Adjudicación única.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    dos AcceptOfferHandler en paralelo sobre la misma línea → exactamente uno won=true, otro
 *               'Ya fue asignada'
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-16 · RNF-16
 * Estado:     Pendiente
 */
