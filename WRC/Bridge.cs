using Windows.ApplicationModel.AppService;

namespace WRC
{
    public static class Bridge
    {
        private static AppServiceConnection Connection { get; set; }
        public static void SetConnection(AppServiceConnection connection)
        {
            Connection = connection;
        }

        public static AppServiceConnection GetConnection()
        {
            return Connection;
        }
    }
}
