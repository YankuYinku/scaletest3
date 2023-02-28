# Build stage
FROM node:lts-alpine as build-stage
WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH
ARG PAT

SHELL [ "/bin/sh", "-c" ]

COPY .npmrc-docker .npmrc
RUN BASE64PAT=`printf "%s" "${PAT}" | base64` \ 
 && sed -i "s|{{AZUREDEVOPS_PAT}}|$BASE64PAT|g" .npmrc

COPY package.json .

RUN npm install

COPY . .

RUN npm run build:webpack


# Production stage
FROM nginx:stable-alpine as production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# copy all possible import maps into the image - select at runtime based on environment variables
COPY src/importmap.Development.json /usr/share/nginx/html 
COPY src/importmap.Staging.json /usr/share/nginx/html 
COPY src/importmap.Production.json /usr/share/nginx/html 
COPY nginx.conf /etc/nginx/nginx.conf

COPY entrypoint.sh /docker-entrypoint.d/40-apetito-settings.sh
RUN chmod +x /docker-entrypoint.d/40-apetito-settings.sh

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]