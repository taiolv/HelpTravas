namespace SuporteIA.Services
{
    public interface IIASuporteService
    {
        Task<string> GerarSugestaoAsync(string descricao);
    }
}
