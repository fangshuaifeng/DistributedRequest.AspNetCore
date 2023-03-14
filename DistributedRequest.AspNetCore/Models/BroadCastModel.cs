using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedRequest.AspNetCore.Models
{
    /// <summary>
    /// 分片模型
    /// </summary>
    public class BroadCastModel
    {
        public BroadCastModel() { }

        public BroadCastModel(int index, int total)
        {
            Index = index;
            Total = total;
        }

        /// <summary>
        /// 分片索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 分片总数
        /// </summary>
        public int Total { get; set; }
    }
}
