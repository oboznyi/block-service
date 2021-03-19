using System;

namespace Block.Sender.DTO
{
    [Serializable]
    public class BlockInfo
    {
        public int BlockNumber { get; set; }
        public DateTime Date { get; set; }
    }
}
