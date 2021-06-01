Docker-compose:
docker-compose -p sks365-ippica-api --env-file development.env up -d

Build:
docker build -t sks365-ippica-api -f src/Sks365.Ippica.Api/Dockerfile . --rm

Run:
docker run -p 6632:80 -e ASPNETCORE_ENVIRONMENT=Development -d --name sks365-ippica-api sks365-ippica-api