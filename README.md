
# Microservices PoC Project

This repository contains a Proof of Concept (PoC) for a microservices architecture with the following services:

- **OrderService.Api**
- **ProductService.Api**
- **PaymentService.Api**
- **UserService.Api**

Each service has its own database and operates independently, while **OrderService.Api** and **PaymentService.Api** communicate asynchronously via Azure Service Bus.

## Prerequisites

Before running the project, make sure you have the following tools installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Azure Subscription](https://azure.microsoft.com/en-us/free/)
- [SQL Server Management Studio (optional)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

### Setting Up Azure Service Bus

The `OrderService.Api` and `PaymentService.Api` integrate with Azure Service Bus for messaging. To set up the Azure Service Bus:

1. Log in to your Azure account using the Azure CLI:

    ```bash
    az login
    ```

2. Create a Service Bus Namespace:

    ```bash
    az servicebus namespace create --name <your-namespace> --resource-group <your-resource-group> --location <your-location>
    ```

3. Create a queue named `orders`:

    ```bash
    az servicebus queue create --name orders --namespace-name <your-namespace> --resource-group <your-resource-group>
    ```

4. Update the connection string in the `appsettings.json` of both **OrderService.Api** and **PaymentService.Api**.

### Setting Up SQL Server in Docker

To run the solution locally, you need a SQL Server database. You can set up SQL Server on your local machine using Docker. Run the following command to set up SQL Server in a Docker container:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=A@a123?321" -p 1433:1433 --name sql1 --hostname sql1 -d mcr.microsoft.com/mssql/server:2022-latest
```

This command will pull the latest SQL Server image from Microsoft and run it in a Docker container with the following configurations:

- **ACCEPT_EULA=Y**: Accepts the SQL Server End-User License Agreement.
- **MSSQL_SA_PASSWORD=A@a123?321**: Sets the password for the SQL Server system administrator (SA) account.
- **-p 1433:1433**: Maps port 1433 from the container to your local machine.
- **--name sql1**: Names the container `sql1`.
- **--hostname sql1**: Sets the hostname to `sql1`.

### Setting Up Databases

Each service has its own database. Instead of manually creating databases, you can run the Entity Framework Core migrations to create and update the databases.

1. Ensure you have the latest **Entity Framework Core tools** installed:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

2. Run the following commands in each of the service projects (starting with **OrderService.Api**, **ProductService.Api**, **PaymentService.Api**, and **UserService.Api**):

    ```bash
    cd OrderService.Api
    dotnet ef database update

    cd ../ProductService.Api
    dotnet ef database update

    cd ../PaymentService.Api
    dotnet ef database update

    cd ../UserService.Api
    dotnet ef database update
    ```

This will create the required databases and apply any existing migrations to keep the schema in sync.

### Running the Services

To run the services locally, navigate to the project folder and use the following commands:

```bash
cd OrderService.Api
dotnet run

cd ProductService.Api
dotnet run

cd PaymentService.Api
dotnet run

cd UserService.Api
dotnet run
```

Each service will start on its own port. Make sure to update any URLs and configurations as needed.

### Running Migrations for Future Schema Updates

Whenever there is a change in the models or the database schema, you should create new migrations for each project by running:

```bash
dotnet ef migrations add <MigrationName> --project <YourServiceProject>
```

Then, run the database update command again:

```bash
dotnet ef database update --project <YourServiceProject>
```

### Testing the Services

#### Endpoints

Each service exposes CRUD endpoints for their respective entities:

- **OrderService.Api**: Manages orders.
- **ProductService.Api**: Manages products.
- **PaymentService.Api**: Manages payments.
- **UserService.Api**: Manages users.

You can use tools like [Postman](https://www.postman.com/) or [curl](https://curl.se/) to interact with the APIs.

#### Messaging

When an order is created in **OrderService.Api**, a message is sent to the `orders` queue in Azure Service Bus. The **PaymentService.Api** listens to this queue and processes the payment for the order.

## Conclusion

This PoC demonstrates a basic microservices architecture using .NET, Azure Service Bus, and SQL Server running in Docker. Each service is independent, with messaging capabilities integrated where necessary.

Feel free to extend or modify the solution based on your needs!
