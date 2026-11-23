# Archivo:    frontend/Dockerfile
# Qué hace:   Compila Angular con Node 22 y lo sirve con nginx 1.27.
# Recibe:     Código de frontend/ (package.json, src/).
# Procesa:    npm ci + ng build (producción); copia dist/ a nginx con default.conf y entrypoint.sh.
# Devuelve:   Contenedor en :80 con basic auth (DEMO_USER / DEMO_PASSWORD) y proxy /api y /hubs al API.
# Se conecta: ← docker-compose.yml (servicio web) · → nginx/default.conf, nginx/entrypoint.sh
# Estado:     Completo
FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN if [ -f package-lock.json ]; then npm ci; else npm install; fi
COPY . .
RUN npx ng build --configuration production

FROM nginx:1.27-alpine
RUN apk add --no-cache apache2-utils
COPY nginx/default.conf /etc/nginx/conf.d/default.conf
COPY nginx/entrypoint.sh /docker-entrypoint.d/10-htpasswd.sh
RUN chmod +x /docker-entrypoint.d/10-htpasswd.sh
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
