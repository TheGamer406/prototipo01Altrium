# Archivo:    graph/Dockerfile
# Qué hace:   Imagen nginx que sirve el grafo del proyecto.
# Recibe:     graph/site/index.html (placeholder hasta la fase 7, luego el HTML generado).
# Procesa:    Copia el sitio, la configuración y el script de basic auth.
# Devuelve:   Contenedor acompana-graph en :80.
# Se conecta: ← docker-compose.yml (servicio graph, perfil extras)
# Estado:     Completo
FROM nginx:1.27-alpine
RUN apk add --no-cache apache2-utils
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY entrypoint.sh /docker-entrypoint.d/10-htpasswd.sh
RUN chmod +x /docker-entrypoint.d/10-htpasswd.sh
COPY site/ /usr/share/nginx/html/
