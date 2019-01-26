using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CredentialManagement;

namespace AdaptivBot
{
    public sealed class CredentialStore
    {
        private static CredentialStore instance = null;
        private static readonly object padlock = new object();
        public Credential credential = new Credential {Target = "AdaptivBot"};
        public bool credentialsFound;

        private CredentialStore()
        {
            credentialsFound = credential.Load();
        }

        public static CredentialStore Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new CredentialStore();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
