/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-invoice.tab.ts
 * Qué hace:   Factura y pago.
 * Recibe:     Detalle + factura.
 * Procesa:    'Enviar al core contable' → 'Enviando…' → aceptada (número del core) o rechazada (motivo
 *               + corregir y reenviar); 'Registrar pago'.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-08 a RF-13
 * Estado:     Pendiente
 */
