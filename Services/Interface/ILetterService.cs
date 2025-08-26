using MessageForAzarab.Models;

namespace MessageForAzarab.Services.Interface
{
    public interface ILetterService
    {
        Task<List<Letter>> GetAllLettersAsync();
        Task<Letter?> GetLetterByIdAsync(int id);
        Task<Letter> CreateLetterAsync(Letter letter);
        Task AddAttachmentAsync(int letterId, Attachment attachment);
        Task UpdateLetterAsync(Letter letter);
        Task DeleteLetterAsync(int id);
    }
}
