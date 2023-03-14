using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedRequest.AspNetCore.Models
{
    public class JobContext
    {
        public JobContext()
        {
        }

        public JobContext(string parameter, BroadCastModel broadCast)
        {
            Parameter = parameter;
            BroadCast = broadCast;
        }

        public string Parameter { get; set; }

        public BroadCastModel BroadCast { get; set; }
    }
}
