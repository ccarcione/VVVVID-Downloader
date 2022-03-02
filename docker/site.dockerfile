FROM node:14 AS builder
WORKDIR /repo
COPY angular-client .
RUN npm install
RUN npm run build -- --prod

FROM nginx AS prod
WORKDIR /site
COPY --from=builder /repo/dist/angular-client .
COPY docker/angular-client-site.conf /etc/nginx/conf.d/default.conf