#!/bin/sh
# Archivo:    graph/entrypoint.sh
# Qué hace:   Genera el archivo de basic auth al arrancar el contenedor.
# Recibe:     Variables de entorno DEMO_USER y DEMO_PASSWORD (desde .env).
# Procesa:    htpasswd -bc /etc/nginx/.htpasswd; falla si faltan; luego agrega los usuarios de DEMO_EXTRA_USERS (usuario:clave,usuario:clave).
# Devuelve:   /etc/nginx/.htpasswd listo antes de que inicie nginx.
# Se conecta: ← graph/Dockerfile · → graph/nginx.conf
# Estado:     Completo
set -e
if [ -z "$DEMO_USER" ] || [ -z "$DEMO_PASSWORD" ]; then
  echo "DEMO_USER y DEMO_PASSWORD son obligatorios" >&2
  exit 1
fi
htpasswd -bc /etc/nginx/.htpasswd "$DEMO_USER" "$DEMO_PASSWORD"
# Usuarios adicionales de muestra: DEMO_EXTRA_USERS="usuario:clave,usuario:clave"
if [ -n "$DEMO_EXTRA_USERS" ]; then
  echo "$DEMO_EXTRA_USERS" | tr ',' '\n' | while IFS=: read -r u p; do
    [ -n "$u" ] && [ -n "$p" ] && htpasswd -b /etc/nginx/.htpasswd "$u" "$p"
  done
fi
