ARG NODE_VERSION=22-alpine
ARG NGINX_VERSION=alpine3.22

FROM node:${NGINX_VERSION} AS builder
WORKDIR /app
COPY package.json package-lock.json* ./
RUN --mount=type=cache,target=/root/.npm npm ci
COPY . .
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build


FROM nginxinc/nginx-unprivileged:${NGINX_VERSION} AS runner
COPY nginx.conf /etc/nginx/nginx.conf
COPY --chown=nginx:nginx --from=builder /app/dist /usr/share/nginx/html
USER nginx
EXPOSE 8080
ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]

