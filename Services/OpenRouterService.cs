using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SuporteIA.Data;
using SuporteIA.Models;

namespace SuporteIA.Services
{
    public class OpenRouterService : IIASuporteService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string? _referer;
        private readonly string? _appName;
        private readonly AppDbContext _ctx;

        private static readonly string[] ModelFallbacks = new[]
        {
            "meta-llama/llama-3.1-8b-instruct:free",
            "mistralai/mistral-7b-instruct:free",
            "gryphe/mythomax-l2-13b:free"
        };

        public OpenRouterService(IConfiguration cfg, AppDbContext ctx)
        {
            _http = new HttpClient();
            _apiKey = cfg["OpenRouter:ApiKey"] ?? throw new Exception("OpenRouter ApiKey não configurada.");
            _referer = cfg["OpenRouter:Referer"];
            _appName = cfg["OpenRouter:AppName"];
            _ctx = ctx;
        }

        public async Task<string> GerarSugestaoAsync(string descricao)
        {
            string? texto = null;
            string? erro = null;

            try
            {
                var endpoint = "https://openrouter.ai/api/v1/chat/completions";

                foreach (var model in ModelFallbacks)
                {
                    try
                    {
                        var body = new
                        {
                            model,
                            messages = new[]
                            {
                                new { role = "system", content = "Você é um técnico de suporte experiente. Sempre responda com texto claro, explicando o diagnóstico e uma possível solução em até 5 linhas. Não use JSON, código ou formatação especial." },
                                new { role = "user", content = $"O usuário relatou: {descricao}. Forneça um diagnóstico provável e uma solução prática." }
                            },
                            temperature = 0.6,
                            max_tokens = 350
                        };

                        _http.DefaultRequestHeaders.Clear();
                        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                        if (!string.IsNullOrWhiteSpace(_referer))
                            _http.DefaultRequestHeaders.Add("HTTP-Referer", _referer);
                        if (!string.IsNullOrWhiteSpace(_appName))
                            _http.DefaultRequestHeaders.Add("X-Title", _appName);

                        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                        var resp = await _http.PostAsync(endpoint, content);
                        var json = await resp.Content.ReadAsStringAsync();

                        Console.WriteLine($"\n=== RESPOSTA {model} ===");
                        Console.WriteLine(json);
                        Console.WriteLine("==========================\n");

                        await File.AppendAllTextAsync("openrouter_log.txt", $"{DateTime.Now}\nMODEL: {model}\n{json}\n\n");

                        if (!resp.IsSuccessStatusCode)
                        {
                            using var edoc = JsonDocument.Parse(json);
                            if (edoc.RootElement.TryGetProperty("error", out var err))
                            {
                                var msgErro = err.GetProperty("message").GetString();
                                Console.WriteLine($"⚠️ Modelo {model} falhou: {msgErro}");
                                continue; 
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Modelo {model} falhou ({(int)resp.StatusCode})");
                                continue;
                            }
                        }

                        using var doc = JsonDocument.Parse(json);
                        if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                        {
                            Console.WriteLine($"⚠️ Modelo {model} não retornou choices válidos.");
                            continue;
                        }

                        var choice = choices[0];

                        if (choice.TryGetProperty("message", out var msgNode) &&
                            msgNode.TryGetProperty("content", out var msgContent))
                        {
                            if (msgContent.ValueKind == JsonValueKind.String)
                            {
                                texto = msgContent.GetString();
                            }
                            else if (msgContent.ValueKind == JsonValueKind.Array)
                            {
                                var sb = new StringBuilder();
                                foreach (var part in msgContent.EnumerateArray())
                                {
                                    if (part.TryGetProperty("text", out var textPart))
                                        sb.AppendLine(textPart.GetString());
                                }
                                texto = sb.ToString();
                            }
                        }

                        if (string.IsNullOrWhiteSpace(texto) && choice.TryGetProperty("text", out var alt))
                            texto = alt.GetString();

                        if (string.IsNullOrWhiteSpace(texto) &&
                            choice.TryGetProperty("delta", out var delta) &&
                            delta.TryGetProperty("content", out var deltaContent))
                            texto = deltaContent.GetString();

                        if (!string.IsNullOrWhiteSpace(texto))
                        {
                            texto = texto
                                .Replace("<s>", "")
                                .Replace("</s>", "")
                                .Replace("[IN]", "")
                                .Replace("[OUT]", "")
                                .Replace("**", "")
                                .Trim();
                        }

                        if (!string.IsNullOrWhiteSpace(texto))
                        {
                            Console.WriteLine($"✅ Modelo usado: {model}");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Modelo {model} retornou resposta vazia. Tentando próximo...");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Falha ao tentar modelo {model}: {ex.Message}");
                        continue;
                    }
                }

                return !string.IsNullOrWhiteSpace(texto)
                    ? texto
                    : "⚠️ Nenhum modelo disponível retornou resposta. Tente novamente mais tarde.";
            }
            catch (Exception ex)
            {
                erro = $"❌ Falha ao consultar o OpenRouter: {ex.Message}";
                return erro;
            }
            finally
            {
                try
                {
                    var log = new IaAuditLog
                    {
                        Provider = "OpenRouter",
                        Prompt = descricao,
                        Response = texto,
                        Error = erro,
                        CreatedAt = DateTime.UtcNow
                    };

                    _ctx.IaAuditLogs.Add(log);
                    await _ctx.SaveChangesAsync();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"⚠️ Falha ao salvar log da IA: {ex2.Message}");
                }
            }
        }
    }
}
