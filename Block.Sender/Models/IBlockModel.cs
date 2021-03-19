using Block.Sender.DTO;
using System.Threading.Tasks;

namespace Block.Sender.Models
{
    public interface IBlockModel
    {
        Task PublishBlockAsync(BlockInfo block);
    }
}
