# README

1. Provision Ubtuntu Server 22.04 LTS Instance. Ports 22 and 5432 must be open. (Port 22 is required for SSH, 5432 for postgres)
2. SSH into instance with SSH credentials.
3. Install Docker & Docker-Compose: 

```
sudo apt-get remove docker docker-engine docker.io containerd runc

sudo apt-get update

sudo apt-get install \
    ca-certificates \
    curl \
    gnupg \
    lsb-release -y

sudo mkdir -p /etc/apt/keyrings

curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update

sudo apt-get install docker-ce docker-ce-cli containerd.io docker-compose-plugin -y

```

[Reference: Install Docker Engine on Ubuntu](https://docs.docker.com/engine/install/ubuntu/)

4. Create or edit ```.env``` file in root folder. This is where database credentials are set via Environment Variables. 
5. Copy root directory into instance. Use a tool like ```scp``` or [WinSCP](https://winscp.net/eng/downloads.php).
6. ```cd``` into ```postgres```.
7. Generate or copy ```server.key``` and ```server.cert``` files into folder

```
# ex:
openssl req -new -text -passout pass:abcd -subj /CN=localhost -out server.req
openssl rsa -in privkey.pem -passin pass:abcd -out server.key
openssl req -x509 -in server.req -text -key server.key -out server.crt
```

8. In root folder ```sudo docker compose up -d --build ``` Runs the containers in detached mode.


## Setup using AWS
1. Login to AWS Management Console 
2. Navigate to EC2 Service.
3. Click Launch Instance.
4. Provide a name for the instance (ie: postgresql-dev)
5. Select an Amazon Machine Image. We will use Ubuntu Server 22.04 LTS. 
6. Select the instance type if you need to change the machine specifications to something other than the free tier eligible instance type.

7. Go to Key pair (login) section. Create new key pair. This will be used to login via SSH. 
   Enter a Key pair name.
   Private key file format depends on your tool of choice/operating system. For PuuTTY on Windows stick with (.pkk) PuTTY
   You will be prompted securely save this file.
8. In the Nework settings section click the Edit button. Then go to the Inbound security groups rules. Click Add security group rule. For the Type of the new rule select PostgreSQL from the drop down. For the Source type select Anywhere from the drop down.
9. Click Launch Instance. The Virtual Server will be online in a few moments. 
10. In the AWS Management Console go back to the EC2 service. Go to instances. Find the instance that was created. Select its checkbox and click the Connect button. AWS will provide details on how to SSH into the machine. The default username for logging into an Ubuntu Server image is "ubuntu".

## References

[The Password File](https://www.postgresql.org/docs/9.3/libpq-pgpass.html)

[pg_dumpall](https://www.postgresql.org/docs/9.1/app-pg-dumpall.html)

[How would I get a cron job to run every 30 minutes?](https://stackoverflow.com/questions/584770/how-would-i-get-a-cron-job-to-run-every-30-minutes)

[Docker Volumes Explained (PostgreSQL example)](https://www.youtube.com/watch?v=G-5c25DYnfI)

[Deploying postgresql docker with ssl certificate and key with volumes](https://stackoverflow.com/questions/55072221/deploying-postgresql-docker-with-ssl-certificate-and-key-with-volumes)