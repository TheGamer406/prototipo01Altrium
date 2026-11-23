#!/bin/sh
# Archivo:    graph/entrypoint.sh
# Qué hace:   Genera el archivo de basic auth al arrancar el contenedor.
# Recibe:     Variables de entorno DEMO_USER y DEMO_PASSWORD (desde .env).
# Procesa:    htpasswd -bc /etc/nginx/.htpasswd; falla si faltan.
# Devuelve:   /etc/nginx/.htpasswd listo antes de que inicie nginx.
# Se conecta: ← graph/Dockerfile · → graph/nginx.conf
# Estado:     Completo
set -e
if [ -z "$DEMO_USER" ] || [ -z "$DEMO_PASSWORD" ]; then
  echo "DEMO_USER y DEMO_PASSWORD son obligatorios" >&2
  exit 1
fi
htpasswd -bc /etc/nginx/.htpasswd "$DEMO_USER" "$DEMO_PASSWORD"
