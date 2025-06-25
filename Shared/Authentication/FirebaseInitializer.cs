// FirebaseInitializer.cs
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
namespace Shared.Authentication;

public static class FirebaseInitializer
{
    private static bool _isInitialized = false;

    public static void InitializeFirebase(string credentialsPath)
    {
        if (_isInitialized) return;

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });

        _isInitialized = true;
    }
}