using System;

namespace Block.Processing.DTO
{
    [Serializable]
    public class BlockInfo
    {
        public int BlockNumber { get; set; }
        public DateTime Date { get; set; }
    }
}
