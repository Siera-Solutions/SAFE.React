FROM alpine:latest

RUN apk --update add postgresql-client

COPY backup.sh /bin/backup.sh
RUN chmod +x /bin/backup.sh

COPY crontabs /etc/crontabs/root

COPY init.sh /bin/init.sh
RUN chmod +x /bin/init.sh

ENTRYPOINT ["ash", "/bin/init.sh"]