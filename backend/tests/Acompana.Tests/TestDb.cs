/*
 * Archivo:    backend/tests/Acompana.Tests/TestDb.cs
 * Qué hace:   Fixture de BD para pruebas.
 * Recibe:     —
 * Procesa:    Levanta PostgreSQL 16 con Testcontainers (necesario para la restricción de exclusión),
 *               migra y siembra lo mínimo.
 * Devuelve:   AppDbContext listo + helpers para crear servicios de prueba.
 * Se conecta: ← todas las pruebas
 * Estado:     Pendiente
 */
