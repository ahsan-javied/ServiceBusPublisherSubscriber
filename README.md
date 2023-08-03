# MessagingService - Service Bus Publisher and Subscriber

Project Details:

1. Azure Cloud Services
	1. Azure subscription
	2. Resource group
	3. SQL Server
	4. SQL Database
	5. Service Bus Namespace
	6. Service Bus Topic
	7. Service Bus Subscription with dead-letter queue

	   a. S1: With default filter
    
	   b. S2: With SQL filter
    
	   c. S3: With correlation filter
     
	9. Function App(s)
     
	   a. Azure Function: Time Trigger
    
	   b. Azure Function: Service Bus Trigger for S1, S2 and S3

2. Backend project in .Net 7 with C#
	1. Microservices:
    
    		a. Time Trigger: Micro service for service bus publisher
    		b. Service Bus Trigger: Micro services for service bus subscribers
	2. DAL:
 
		a. Dapper with singelton design pattern to connection with DB

	   	b. Generic Repository pattern for CRUD opperations
	4. Services: Data mapper and manipulator brigde between layers
	5. Utils: Utilities and helpers of reuseable functions

3. Database
	1. DB
	2. Table(s)
	3. Storeprocedure(s) for DML opperations
	
