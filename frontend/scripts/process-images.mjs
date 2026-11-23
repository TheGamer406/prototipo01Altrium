/**
 * Archivo:    frontend/scripts/process-images.mjs
 * Qué hace:   Convierte las imágenes entregadas a .webp para la app (npm run assets).
 * Recibe:     ../assets/source/{cofres,florales,otros}/*.png
 * Procesa:    sharp: resize({ width: 800, withoutEnlargement: true }) → nunca agranda (las fuentes son de
 *             ~340 px); .webp calidad 82; renombra vleorioMuestra1 → velorioMuestra1.
 * Devuelve:   src/assets/img/{cofres,florales,otros}/*.webp (cofreN, floresN, cateringN, mariachiN,
 *             velorioMuestraN).
 * Se conecta: → catálogo (image_path del seed), paso 4 del asistente, tarjetas de paquete, estados vacíos
 * Estado:     Pendiente (fase 3)
 */
