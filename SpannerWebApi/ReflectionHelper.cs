using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using System.Reflection;

namespace SpannerWebApi;

public static class ReflectionHelper
{
    public static SessionPool GetSessionPool(this SpannerConnection connection)
    {
        // SpannerConnection has a field called _sessionPool.
        var field = typeof(SpannerConnection).GetField("_sessionPool", BindingFlags.NonPublic | BindingFlags.Instance);
        return (SessionPool)field.GetValue(connection);
    }

    // This is indeed hacky but very easy and quick to do to try out the steps and reproduce the bug.
    public static SpannerClient GetSpannerClient(this SpannerConnection connection)
    {
        var pool = connection.GetSessionPool();
        if (pool != null)
        {
            // SessionPool has a property called Client.
            var property = typeof(SessionPool).GetProperty("Client", BindingFlags.NonPublic | BindingFlags.Instance);
            return (SpannerClient)property.GetValue(pool);
        }

        return default;
    }
}
