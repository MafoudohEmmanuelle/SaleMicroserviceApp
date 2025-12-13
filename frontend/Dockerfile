# Install Node
FROM node:24-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Serve with Vite preview
FROM node:20-alpine AS preview
WORKDIR /app
COPY --from=build /app/dist ./dist
RUN npm install -g vite
EXPOSE 4173
CMD ["vite", "preview", "--port", "4173", "--host"]
