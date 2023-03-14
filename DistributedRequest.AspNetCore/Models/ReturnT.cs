namespace DistributedRequest.AspNetCore.Models
{
    public class ReturnT
    {
        public const int SuccessCode = 200;

        public const int FailCode = 500;

        public int Code { get; set; }

        public string Msg { get; set; }

        public object Data { get; set; }

        public ReturnT()
        {
        }

        public ReturnT(int code, object data, string msg)
        {
            Code = code;
            Data = data;
            Msg = msg;
        }

        public static ReturnT Failed(string msg)
        {
            return new ReturnT(FailCode, null, msg);
        }

        public static ReturnT Success(object data, string msg = null)
        {
            return new ReturnT(SuccessCode, data, msg);
        }
    }
}
