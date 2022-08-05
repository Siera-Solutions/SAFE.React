FROM postgres:14.3-alpine

COPY server.key /var/lib/postgresql/server.key
COPY server.crt /var/lib/postgresql/server.crt

RUN chmod 600 /var/lib/postgresql/server.key
RUN chown 70:70 /var/lib/postgresql/server.key