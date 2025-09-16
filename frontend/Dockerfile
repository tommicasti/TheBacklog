# Use official Node.js LTS image
FROM node:18-alpine

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm install --production

# Copy the rest of the application code
COPY . .

# Build the React app
RUN npm run build

# Use a lightweight web server to serve the build
RUN npm install -g serve

# Expose port 3000
EXPOSE 3000

# Start the app using 'serve'
CMD ["serve", "-s", "build", "-l", "3000"]
