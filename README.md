# DynamicSearch
One service to connect to any database, and then easy perform a search for complex relational data.

# Shot story
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

