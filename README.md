Azure sql issue:
For Azure sql you should run: appsetting.json 
But ASPNETCORE_ENVIRONMENT is set to Development. It is set in VS->r.Click proj->properties->'Debug' tab
So when run update-database for azure sql, appsettings.development.json file have to move or rename, otherwise update-database command will run appsettings.development.json file, instead appsettings.json file