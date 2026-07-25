# Build from repo root: docker build -f AdminPanelUI/Dockerfile .
FROM node:22-alpine AS build
WORKDIR /app

COPY AdminPanelUI/package.json AdminPanelUI/package-lock.json* ./
RUN npm ci

COPY AdminPanelUI/ ./
RUN npm run build

FROM nginx:1.27-alpine AS final
COPY docker/nginx/spa.conf /etc/nginx/conf.d/default.conf
COPY docker/nginx/40-runtime-config.sh /docker-entrypoint.d/40-runtime-config.sh
RUN chmod +x /docker-entrypoint.d/40-runtime-config.sh
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
