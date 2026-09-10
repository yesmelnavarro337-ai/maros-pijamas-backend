using Maros.Application.DTOs.Home;

namespace Maros.Application.Interfaces;

public interface IHomeService
{
    Task<HomePageDto> GetHomeAsync();
}