using Entities;
using Models.RequestModels;
using System.Threading.Tasks;

public interface ILoginService
{
    Task<LoginResponse> LoginAsync(string email, string password);
}