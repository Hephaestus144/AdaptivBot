using CredentialManagement;


namespace AdaptivBot
{
    public sealed class CredentialStore
    {
        public Credential credential;
        public bool credentialsFound;

        public CredentialStore(string target)
        {
            credential = new Credential { Target = target };
            credentialsFound = credential.Load();
        }
    }
}
