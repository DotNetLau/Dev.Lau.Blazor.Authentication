# Dev.Lau.Blazor.Authentication

The example Blazor server app connects to Microsoft Entra ID to authenticate.

## App Registration config

- Add a platform: Web;
  - Add a redirect URL, e.g.: [https://localhost:7198/signin-oidc](https://localhost:7198/signin-oidc).
- Enable ID tokens;
- Under API Permissions, add: Microsft Graph -> User.Read (don't forget to grant permission for it);
- Configure App Roles if you want (assign the role in the Enterprise Application, not the App Registration).

## Appsettings.json

Change the values to your tenant and App Registration's (or Enterprise Application (they are the same)) client/application ID.
