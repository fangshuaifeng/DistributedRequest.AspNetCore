namespace DistributedRequest.AspNetCore.Models
{
    public class ReturnT<T>
    {
        public const int SuccessCode = 200;

        public const int FailCode = 500;

        public int Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public ReturnT()
        {
        }

        public ReturnT(int code, T data, string msg)
        {
            Code = code;
            Data = data;
            Msg = msg;
        }

        public static ReturnT<T> Failed(string msg)
        {
            return new ReturnT<T>(FailCode, default, msg);
        }

        public static ReturnT<T> Success(T data, string msg = null)
        {
            return new ReturnT<T>(SuccessCode, data, msg);
        }
    }

    public class ReturnT : ReturnT<object>
    {
        public ReturnT(int code, object data, string msg) : base(code, data, msg) { }

        public static ReturnT Failed(string msg)
        {
            return new ReturnT(FailCode, default, msg);
        }

        public static ReturnT Success(object data, string msg = null)
        {
            return new ReturnT(SuccessCode, data, msg);
        }
    }
}
