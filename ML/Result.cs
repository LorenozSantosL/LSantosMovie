namespace ML
{
    public class Result
    {
        public bool Correct { get; set; }
        public string Message { get; set; }
        public Exception EX { get; set; }
        public Object Object { get; set; }
        public List<object> Objects { get; set; }
    }
}