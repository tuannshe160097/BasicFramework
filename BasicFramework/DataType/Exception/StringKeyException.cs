namespace BasicFramework.DataType.Exception
{
    internal class StringKeyException : System.Exception
    {
        public StringKeyException() : base("Cannot read value from object using a string key") { }
    }
}
