/*
 * Archivo:    backend/tests/Acompana.Tests/TaxExemptionTests.cs
 * Qué hace:   Prueba xUnit: Cálculo de exoneraciones.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    líneas con 0 %, 50 % y 100 % exento → subtotal, exento, IVA 13 % y total esperados
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-10
 * Estado:     Pendiente
 */
