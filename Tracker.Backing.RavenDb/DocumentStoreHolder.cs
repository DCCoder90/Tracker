using Raven.Client.Documents;

namespace Tracker.RavenDb;

internal class DocumentStoreHolder
{
    // Use Lazy<IDocumentStore> to initialize the document store lazily. 
    // This ensures that it is created only once - when first accessing the public `Store` property.

    public static IDocumentStore Store { get; } = CreateStore();

    private static IDocumentStore CreateStore()
    {
        var store = new DocumentStore
        {
            // Define the cluster node URLs (required)
            Urls = new[] { "http://127.0.0.1:8080" },

            // Set conventions as necessary (optional)
            Conventions =
            {
                MaxNumberOfRequestsPerSession = 10,
                UseOptimisticConcurrency = true
            },

            // Define a default database (optional)
            Database = "test"

            // Define a client certificate (optional)
            //Certificate = new X509Certificate2("C:\\path_to_your_pfx_file\\cert.pfx"),

            // Initialize the Document Store
        }.Initialize();

        // When the store is disposed of, the certificate file will be removed as well
        //Store.AfterDispose += (sender, args) => Store.Certificate.Dispose();

        return store;
    }
}