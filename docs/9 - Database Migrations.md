# 9: Database Migrations

### 9.1 Running Migrations

1. 1. Copy the migrations folder from Poort8.Dataspace.(_Organization/Authorization_)Registry.SqliteMigrations, depending on whether you changed Organization registry or Authorization registry data models.
2. In the AddOrganizationRegistrySqlite method in the Poort8.Dataspace.(_Organization/Authorization_)Registry.Extensions.DefaultExtension class, change the MigrationsAssembly from 'Poort8.Dataspace.(_Organization/Authorization_)Registry.SqliteMigrations' to 'Poort8.Dataspace.CoreManager'. This points the startup process to migration files of step 1.
3. Execute entity framework command to generate the migration script:
 * Package Manager Console:
```bash
Add-Migration SuitableNameForYourChange -Context (Organization/Authorization)Context
```
 * CLI:
```bash
dotnet ef migrations add SuitableNameForYourChange -c (Organization/Authorization)Context
```
4. Copy the new migration .cs and .Designer.cs files (named _SuitableNameForYourChange_) created in the folder from step 1 along with the (_Organization/Authorization_)ContextModelSnapshot.cs from that folder to the original migrations folder.
5. Revert the name change of step 2.
6. Comment the Add(_Organization/Authorization_)RegistrySqlite lines in Program.cs of the Poort8.Dataspace.CoreManager and uncomment the Add(_Organization/Authorization_)RegistrySqlServer lines.
7. Repeat step 1 to 5 but now for the SqlServerMigrations project instead of the SqliteMigrations.
8. Revert the changes from step 6.