# Troubleshooting

Common issues and fixes.

## Build fails on restore
- Ensure the correct .NET SDK is installed
- Run `dotnet --info` and verify versions
- Clear caches: `dotnet nuget locals all --clear`

## Database objects missing
- Re?run `Zzz_Deploy` scripts
- Verify connection string in `AppEnd/AppEndHost/appsettings.json`

## Cannot login
- Ensure default credentials (Admin / P#ssw0rd) for initial run
- Check logs for authentication issues

## API returns 500
- Review application logs
- Validate required settings in configuration
- Confirm database connectivity

## UI not loading
- Check browser console for errors
- Ensure APIs are reachable (CORS, base URL)

If issues persist, open an issue with logs and reproduction steps.