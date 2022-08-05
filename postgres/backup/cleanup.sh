#!/bin/ash
find /var/opt/backups/ -type f -mtime +1 -maxdepth 1 -exec rm {} \;