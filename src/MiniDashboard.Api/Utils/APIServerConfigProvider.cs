using MiniDashboard.DataAccess;

namespace MiniDashboard.Api.Utils
{
    public class APIServerConfigProvider : IConfigProvider
    {
        public string GetProductStoreFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "datastore.json");
        }
    }
}
