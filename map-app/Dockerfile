FROM node:21.7.3-alpine
WORKDIR /app
COPY public/ public/
COPY src/ src/
COPY package.json .
RUN npm install
CMD ["npm", "start"]