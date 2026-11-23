/*
 * Archivo:    backend/tests/Acompana.Tests/DeathCertificateRuleTests.cs
 * Qué hace:   Prueba xUnit: Acta antes de facturar.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    servicio inmediato sin acta → IssueInvoice lanza 'Registre el acta…'; con acta → pasa a
 *               Invoicing
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-03
 * Estado:     Pendiente
 */
