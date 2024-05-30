using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;
using Tracker.Net.Repository;

namespace Tracker.RavenDb;

internal class DocumentStoreHolder
{
    private static Lazy<IDocumentStore> _store;

    public static IDocumentStore GetStore(BackingOptions backingOptions)
    {
            if (_store==null || !_store.IsValueCreated)
                _store = new Lazy<IDocumentStore>(CreateStore(backingOptions));
            return _store.Value;
    }

    private static IDocumentStore CreateStore(BackingOptions BackingOptions)
    {
        if (BackingOptions == null)
            throw new ArgumentNullException(nameof(BackingOptions),"BackingOptions cannot be null");
        
        var store = new DocumentStore
        {
            // Define the cluster node URLs (required)
            Urls = new[] { $"{BackingOptions.Host}:{BackingOptions.Port}" },

            // Set conventions as necessary (optional)
            Conventions =
            {
                MaxNumberOfRequestsPerSession = 10,
                UseOptimisticConcurrency = true
            },

            Database = BackingOptions.Database,
            Certificate = BackingOptions.UsesAuthentication ? new X509Certificate2(BackingOptions.CertificatePath) : null,
        }.Initialize();

        // When the store is disposed of, the certificate file will be removed as well
        if(BackingOptions.UsesAuthentication)
            store.AfterDispose += (sender, args) => store.Certificate.Dispose();

        return store;
    }
}