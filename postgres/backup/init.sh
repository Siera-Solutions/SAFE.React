#!/bin/ash
echo $POSTGRES_HOST:5432:$POSTGRES_DB:$POSTGRES_USER:$POSTGRES_PASSWORD >> /root/.pgpass
chmod 600 /root/.pgpass
export PGPASSFILE=/root/.pgpass
export PGPASSWORD=$POSTGRES_PASSWORD
export PATH=$PATH:/root
crond -f -d 8