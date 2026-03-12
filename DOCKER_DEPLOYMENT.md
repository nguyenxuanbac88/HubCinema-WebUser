# HubCinema WebUser - Docker Deployment Guide

## H??ng d?n build và push Docker image lên Docker Hub

### B??c 1: ??ng nh?p Docker Hub
```bash
docker login
```
Nh?p username và password c?a b?n.

### B??c 2: Build Docker image
```bash
# Thay YOUR_DOCKERHUB_USERNAME b?ng username Docker Hub c?a b?n
docker build -t YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest .
```

Ví d?:
```bash
docker build -t nguyenxuanbac88/hubcinema-webuser:latest .
```

### B??c 3: Ki?m tra image ?ã build
```bash
docker images | grep hubcinema-webuser
```

### B??c 4: Test image locally (tùy ch?n)
```bash
docker run -d -p 8080:8080 --name hubcinema-test YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest
```

Truy c?p: http://localhost:8080

D?ng và xóa container test:
```bash
docker stop hubcinema-test
docker rm hubcinema-test
```

### B??c 5: Push image lên Docker Hub
```bash
docker push YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest
```

### B??c 6: Tag thêm version (tùy ch?n)
```bash
# Tag v?i version c? th?
docker tag YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:v1.0.0

# Push version tag
docker push YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:v1.0.0
```

## S? d?ng Docker Compose

### Build và run v?i Docker Compose:
```bash
# Set username Docker Hub c?a b?n
export DOCKER_USERNAME=YOUR_DOCKERHUB_USERNAME

# Build và start
docker-compose up -d --build

# Xem logs
docker-compose logs -f

# Stop
docker-compose down
```

## Pull và run t? Docker Hub

Sau khi ?ã push lên Docker Hub, ng??i khác có th? pull và run:

```bash
docker pull YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest
docker run -d -p 8080:8080 YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest
```

## Commands h?u ích

### Xem container ?ang ch?y
```bash
docker ps
```

### Xem logs
```bash
docker logs <container_id>
```

### Vào trong container
```bash
docker exec -it <container_id> /bin/bash
```

### Xóa image
```bash
docker rmi YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest
```

### Xóa t?t c? containers ?ã d?ng
```bash
docker container prune
```

### Xóa t?t c? images không s? d?ng
```bash
docker image prune -a
```

## L?u ý quan tr?ng

1. **appsettings.json**: ??m b?o c?u hình production ?úng, ??c bi?t là connection strings và API keys
2. **Environment Variables**: Có th? override c?u hình qua environment variables khi run container
3. **Port**: Image expose port 8080 m?c ??nh
4. **QRCoder**: ?ã cài ??t libgdiplus ?? h? tr? QRCoder library

## Ví d? v?i environment variables

```bash
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING" \
  YOUR_DOCKERHUB_USERNAME/hubcinema-webuser:latest
```

## Automation Script (build-and-push.sh)

T?o file script ?? t? ??ng hóa:

```bash
#!/bin/bash

DOCKER_USERNAME="YOUR_DOCKERHUB_USERNAME"
IMAGE_NAME="hubcinema-webuser"
VERSION="latest"

echo "Building Docker image..."
docker build -t $DOCKER_USERNAME/$IMAGE_NAME:$VERSION .

echo "Tagging with version..."
docker tag $DOCKER_USERNAME/$IMAGE_NAME:$VERSION $DOCKER_USERNAME/$IMAGE_NAME:v1.0.0

echo "Pushing to Docker Hub..."
docker push $DOCKER_USERNAME/$IMAGE_NAME:$VERSION
docker push $DOCKER_USERNAME/$IMAGE_NAME:v1.0.0

echo "Done!"
```

Ch?y script:
```bash
chmod +x build-and-push.sh
./build-and-push.sh
```
