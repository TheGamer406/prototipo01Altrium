/*
 * Archivo:    frontend/scripts/process-images.mjs
 * Qué hace:   Convierte las imágenes entregadas a .webp y copia las fuentes self-hosted (npm run assets).
 * Recibe:     ../assets/source/{cofres,florales,otros}/*.png · node_modules/@fontsource/inter y material-symbols
 * Procesa:    sharp: resize({ width: 800, withoutEnlargement: true }) → nunca agranda (las fuentes son de
 *             ~340 px); .webp calidad 82; renombra vleorioMuestra1 → velorioMuestra1.
 *             Copia Inter (400, 500, 600) y Material Symbols Outlined a public/fonts/.
 * Devuelve:   public/img/{cofres,florales,otros}/*.webp (cofreN, floresN, cateringN, mariachiN,
 *             velorioMuestraN) y public/fonts/*.woff2 (se sirven en /img/... y /fonts/...).
 * Se conecta: → catálogo (image_path del seed), paso 4 del asistente, tarjetas de paquete, estados vacíos
 * Estado:     Completo
 */
import { mkdir, readdir, copyFile } from 'node:fs/promises';
import { join, parse, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';
import sharp from 'sharp';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const source = join(root, '..', 'assets', 'source');
const out = join(root, 'public', 'img');

for (const folder of ['cofres', 'florales', 'otros']) {
  const dir = join(source, folder);
  await mkdir(join(out, folder), { recursive: true });
  for (const file of await readdir(dir)) {
    if (!file.toLowerCase().endsWith('.png')) continue;
    const name = parse(file).name.replace('vleorioMuestra', 'velorioMuestra');
    await sharp(join(dir, file))
      .resize({ width: 800, withoutEnlargement: true })
      .webp({ quality: 82 })
      .toFile(join(out, folder, `${name}.webp`));
  }
}

const fonts = join(root, 'public', 'fonts');
await mkdir(fonts, { recursive: true });
for (const w of [400, 500, 600]) {
  await copyFile(join(root, 'node_modules/@fontsource/inter/files', `inter-latin-${w}-normal.woff2`), join(fonts, `inter-${w}.woff2`));
}
await copyFile(join(root, 'node_modules/material-symbols/material-symbols-outlined.woff2'), join(fonts, 'material-symbols-outlined.woff2'));
console.log('Imágenes y fuentes listas en public/');
