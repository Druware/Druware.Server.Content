# Druware.Server.Content

The Content namespace contains all of the relevant bits to enable a simple but
effective CMS within the context of an application that includes it.


How to add new fields to an entity


Command Lines to build new migrations when fields are added
```aiignore
dotnet ef migrations add article-v2 --output-dir Migrations/Microsoft --context ContentContextMicrosoft
dotnet ef migrations add article-v2 --output-dir Migrations/PostgreSql --context ContentContextPostgreSql
dotnet ef migrations add article-v2 --output-dir Migrations/Sqlite --context ContentContextSqlite
```