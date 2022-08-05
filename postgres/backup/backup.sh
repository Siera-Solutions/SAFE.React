#!/bin/ash
pg_dumpall --no-password -h $POSTGRES_HOST -p 5432 -U $POSTGRES_USER >> /var/opt/$(date +"%Y%m%d%H%M%S").out