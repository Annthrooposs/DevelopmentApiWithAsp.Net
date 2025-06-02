
using TP2_AméliorerlaMinimalAPIduTP1.DTO;



namespace TP2_AméliorerlaMinimalAPIduTP1.Services;

public interface ITodoService
{
     Task<List<TodoOutputDto>> GetAll();
     Task<TodoOutputDto?> GetById(int id);
     Task<TodoOutputDto> Add(TodoInputDto person);
     Task<bool> Update(int id, TodoInputDto person);
     Task<bool> Delete(int id);
}
