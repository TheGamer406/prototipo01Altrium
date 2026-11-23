/*
 * Archivo:    backend/src/Acompana.Application/Meta/MetaQuery.cs
 * Qué hace:   Datos para selectores de la interfaz.
 * Recibe:     nada.
 * Procesa:    Sedes (id, code, name, zone), roles demo con etiqueta en español, enums con traducción.
 * Devuelve:   { branches[], roles[{value, label}], enums{name: [{value, label}]} }.
 * Se conecta: ← GET /api/meta/* · → selectores de sede y rol
 * Reglas:     RF-30
 * Estado:     Pendiente
 */
