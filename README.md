# DynamicSearch
One service to connect to any database, and then easy perform a search for complex relational data.

# Short story
The service read configuration file, and uses a Entity Framework Scaffold options (Reverse Engineering) to creates Contexts, Entities, DbSets and 
temporary (until service is alive) store them into memory. Then a full search of loaded entities is exposed trough REST API.

# Configuration file

Bellow is json represent of configuration file:

```json
  "Clients": [
      {
        "ConnectionString": "Server=localhost,1433; Database=bp2_scaffold; User Id=sa; Password=****; TrustServerCertificate=True; MultipleActiveResultSets=true;",
        "Name": "Client1",
        "SchemaNames": ["dbo", "bp2"],
        "IncludeTables": []
      }
      {
        "ConnectionString": "Server=localhost,1433; Database=bp1; User Id=sa; Password=***; TrustServerCertificate=True; MultipleActiveResultSets=true;",
        "Name": "Client2",
        "SchemaNames": [ "bp2" ],
        "IncludeTables": [studentTable]
      }
    ]
```

Json file consists of an array of Clients. Bellow is table whith explanation of Client properties:
|    Property name      | Description     |  Additional description|
| ------------- | ------------- | -------------
|       ConnectionString     | An expression that contains the parameters required for the applications to connect a database server   | /|
|       Name    | Unique Client name.   | Only one uniqu name in entire array of Clients.|
|       SchemaNames    | Database schema names for service to search and include in scaffold process.  | Empty array to include all.|
|       IncludeTables    | Database table names for service to search and include in scaffold process.  |Empty array to include all.|


