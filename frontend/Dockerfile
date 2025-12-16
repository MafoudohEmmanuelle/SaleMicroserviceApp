# Install Node
FROM node:24-alpine AS build
WORKDIR /app
ARG VITE_SALES_API_URL
ARG VITE_ITEMS_API_URL
ARG VITE_USERS_API_URL
ENV VITE_SALES_API_URL=$VITE_SALES_API_URL
ENV VITE_ITEMS_API_URL=$VITE_ITEMS_API_URL
ENV VITE_USERS_API_URL=$VITE_USERS_API_URL
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
