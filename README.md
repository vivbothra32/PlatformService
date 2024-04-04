Hello People! 
This is my first .Net 6.0 Microservice Architecture based project wherein I have created two services:
1. PlatformService - Users can add/delete/modify information about various Platforms(programs/technologies in use).
2. CommandService - Commands specific to the Platforms added are managed using this microservice.

Major implementations achieved using this project:
1. Synchronous communication between the two services - using HTTPClient.
2. Asynchronous communication - using RabbitMQ Message Bus.
3. Database - MSSQL Server 2020.
4. Containerization of the microservices - using Docker.
5. Container orchestration - using Kubernetes.

     a. Services deployment.

     b. MSSQL server with Persistent Volume Storage

     c. RabbitMQ - publisher/subscriber model with FanOut exchange implementation.

     d. Ingress-Nginx controller - routing requests to the different services.

Link to the DockerHub repository - [here](https://hub.docker.com/repositories/vivbothra32).

Link to the K8S deployment files - [here](https://github.com/vivbothra32/K8S).

Link to the other microservice - CommandService - [here](https://github.com/vivbothra32/CommandsService).
