/*
 * Archivo:    backend/src/Acompana.Application/Oversight/OverviewQuery.cs
 * Qué hace:   Vista central: todas las sedes con servicios en curso, capillas ocupadas e inventario.
 * Recibe:     ICurrentUser (Management ve todo; otro rol solo su sede).
 * Procesa:    Lee analytics.mv_branch_snapshot y la completa con alertas abiertas en vivo.
 * Devuelve:   OverviewDto.
 * Se conecta: ← GET /api/overview · se refresca con eventos del hub
 * Reglas:     RF-29 · RNF-06
 * Estado:     Pendiente
 */
