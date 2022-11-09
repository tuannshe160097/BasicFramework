namespace BasicFramework.Http
{
    public static class HttpStatus
    {
        public static readonly Dictionary<int, string> status = new Dictionary<int, string>()
        {
            {200, "OK" },
            {404, "Not Found" },
            {500, "Internal Server Error" },
        };
    }
}
