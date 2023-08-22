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

# API
A more detailed description is described via the [open api specification](apiDocs/openApiSpec.yaml)

# How it work?
To understand how the service returns data, let's assume that we have a simple database with several tables.
Below is an image with a base consisting of up to three tables Authors, Books, and BookCopies:

![image](https://github.com/JovanDodic89/DynamicSearch/assets/98430280/ee646b23-e606-48db-9606-01cf89f7e172)

Lets assume that we have configurated our configuration json to scaffold this schema and tables. Such a json would look like this:
```json
  "Clients": [
      {
        "ConnectionString": "Server=localhost,1433; Database=bp2_scaffold; User Id=sa; Password=****; TrustServerCertificate=True; MultipleActiveResultSets=true;",
        "Name": "library",
        "SchemaNames": ["dbo"],
        "IncludeTables": ["Authors","Books", "BookCopies"]
      }
    ]
```
Below are examples of client table search URLs with possible answers:
 1. api/v1/search/library/dbo/Authors?page=1&pageLimit=200
    - Response 200 OK 
    ```json
    {
        "totalCount": 3,
        "page": 1,
        "pageLimit": 200,
        "items": [
            {
                "authorId": 1,
                "authorName": "J.K. Rowling",
                "books": []
            },
            {
                "authorId": 2,
                "authorName": "George Orwell",
                "books": []
            },
            {
                "authorId": 3,
                "authorName": "Jane Austen",
                "books": []
            }
        ]
    }
    ```
2. api/v1/search/library/dbo/Authors?**page=0**pageLimit=200
    - Response 400 BadRequest
    ```json
        {
        "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "errors": {
            "Page": [
                "'Page' must be greater than or equal to '1'."
            ]
        }
    }
    ```
 3. To perform search for specific records when can use a **searchString** from third part [library](https://github.com/JovanDodic89/SearchByStringExtensions)

    api/v1/search/library/dbo/Books?page=1&pageLimit=200&**searchString=titlestartswithHarry**

    - Response 200 OK
      ```json
          {
          "totalCount": 1,
          "page": 1,
          "pageLimit": 200,
          "items": [
              {
                  "bookId": 1,
                  "title": "Harry Potter and the Sorcerer's Stone",
                  "authorId": 1,
                  "author": null,
                  "bookCopies": []
              }
          ]
      }
      ```
