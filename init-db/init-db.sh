#!/bin/bash

until /opt/mssql-tools/bin/sqlcmd -S mssql -U sa -P 'SuperStrong!Passw0rd' -Q 'SELECT 1'; do
  >&2 echo "SQL Server is unavailable"
  sleep 1
done

>&2 echo "SQL Server is up - executing script"

/opt/mssql-tools/bin/sqlcmd -S mssql -U sa -P 'SuperStrong!Passw0rd' -i /docker-entrypoint-initdb.d/init-db.sql

echo "init-db.sql script has been executed successfully"