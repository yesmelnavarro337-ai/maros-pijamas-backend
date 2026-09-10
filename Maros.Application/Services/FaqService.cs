using Maros.Application.Common;
using Maros.Application.DTOs.Faq;
using Maros.Application.Interfaces;
using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Domain.Interfaces;

namespace Maros.Application.Services;

public class FaqService : IFaqService
{
    private readonly IFaqRepository _repository;

    public FaqService(IFaqRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FaqResponseDto>> GetAllAsync()
    {
        var faqs = await _repository.GetAllOrderedAsync();
        return faqs.Select(ToDto).ToList();
    }

    public async Task<FaqResponseDto> CreateAsync(FaqCreateDto request)
    {
        var status = ParseStatus(request.Status);
        var order = await _repository.GetNextOrderAsync();

        var faq = new Faq
        {
            Question = request.Question,
            Answer = request.Answer,
            Category = request.Category,
            Status = status,
            Order = order,
        };

        await _repository.AddAsync(faq);
        await _repository.SaveChangesAsync();

        return ToDto(faq);
    }

    public async Task<FaqResponseDto> UpdateAsync(Guid id, FaqUpdateDto request)
    {
        var faq = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        faq.Question = request.Question;
        faq.Answer = request.Answer;
        faq.Category = request.Category;
        faq.Status = ParseStatus(request.Status);
        faq.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(faq);
    }

    public async Task ReorderAsync(Guid id, string direction)
    {
        if (direction != "up" && direction != "down")
            throw new AppException("Dirección de reordenamiento inválida.", 400);

        var faqs = await _repository.GetAllOrderedAsync();
        var index = faqs.FindIndex(f => f.Id == id);
        if (index == -1)
            throw new AppException("Pregunta no encontrada.", 404);

        var targetIndex = direction == "up" ? index - 1 : index + 1;
        if (targetIndex < 0 || targetIndex >= faqs.Count)
            return; // ya está en el extremo, no hay nada que reordenar

        (faqs[index].Order, faqs[targetIndex].Order) = (faqs[targetIndex].Order, faqs[index].Order);

        await _repository.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var faq = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Pregunta no encontrada.", 404);

        _repository.Remove(faq);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<FaqPublicDto>> GetPublicAsync()
    {
        var faqs = await _repository.GetPublicOrderedAsync();
        return faqs.Select(f => new FaqPublicDto(f.Question, f.Answer, f.Category)).ToList();
    }

    private static FaqStatus ParseStatus(string input)
    {
        if (!Enum.TryParse<FaqStatus>(input, ignoreCase: true, out var status))
            throw new AppException("Estado de pregunta inválido.", 400);
        return status;
    }

    private static FaqResponseDto ToDto(Faq f) =>
        new(f.Id, f.Question, f.Answer, f.Category, f.Order, f.Status.ToString());
}