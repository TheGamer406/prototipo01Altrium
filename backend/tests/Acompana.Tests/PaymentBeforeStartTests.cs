/*
 * Archivo:    backend/tests/Acompana.Tests/PaymentBeforeStartTests.cs
 * Qué hace:   Prueba xUnit: Cobro antes de iniciar.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    sin pago no se despachan ofertas ni se asigna personal; tras pagar sí
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-08
 * Estado:     Pendiente
 */
