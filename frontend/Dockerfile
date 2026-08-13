# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps
COPY . .
RUN npm run build -- --configuration production

# Stage 2: Serve
FROM nginx:alpine
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
# Copy custom nginx config template
COPY nginx.conf.template /etc/nginx/templates/default.conf.template
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
