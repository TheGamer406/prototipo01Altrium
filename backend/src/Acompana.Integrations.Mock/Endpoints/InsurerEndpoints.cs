/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/InsurerEndpoints.cs
 * Qué hace:   Simula la recepción del aviso estructurado de la aseguradora.
 * Recibe:     POST /aseguradoras/notices { insurer, policy, insured, serviceCode, amountCrc }.
 * Procesa:    Valida campos obligatorios.
 * Devuelve:   202 · 400.
 * Se conecta: ← InsurerClient
 * Reglas:     RF-12
 * Estado:     Pendiente
 */
