#!/bin/sh
# Archivo:    scripts/create-demo-users.sh
# Qué hace:   Crea en pgAdmin los usuarios administradores de muestra definidos en .env.
# Recibe:     PGADMIN_EXTRA_USERS="correo:clave,correo:clave" en .env y el contenedor acompana-pgadmin en marcha.
# Procesa:    Por cada par ejecuta setup.py add-user --admin dentro del contenedor; si el usuario ya existe lo
#             actualiza con la clave indicada.
# Devuelve:   Usuarios creados o actualizados (mensaje por usuario).
# Se conecta: ← docker-compose.yml (servicio pgadmin) · se ejecuta una vez tras levantar el perfil extras.
# Estado:     Completo
set -e
cd "$(dirname "$0")/.."
LIST=$(grep '^PGADMIN_EXTRA_USERS=' .env | cut -d= -f2-)
[ -z "$LIST" ] && { echo "PGADMIN_EXTRA_USERS vacío en .env"; exit 0; }
echo "$LIST" | tr ',' '\n' | while IFS=: read -r email pass; do
  [ -z "$email" ] && continue
  docker exec acompana-pgadmin /venv/bin/python3 /pgadmin4/setup.py add-user "$email" "$pass" --admin >/dev/null 2>&1 \
    && echo "creado: $email" \
    || { docker exec acompana-pgadmin /venv/bin/python3 /pgadmin4/setup.py update-user "$email" --password "$pass" >/dev/null 2>&1 && echo "actualizado: $email" || echo "ERROR con $email"; }
done
