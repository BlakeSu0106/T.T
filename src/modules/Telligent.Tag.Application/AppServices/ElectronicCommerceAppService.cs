using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telligent.Core.Infrastructure.Services;
using Telligent.Tag.Application.Configs;
using Telligent.Tag.Application.Dtos.ElectronicCommerce;

namespace Telligent.Tag.Application.AppServices;

public class ElectronicCommerceAppService : IAppService
{
    private readonly Config _config;

    public ElectronicCommerceAppService(IOptions<Config> config)
    {
        _config = config.Value;
    }

    public async Task<List<ProductDto>> GetProductListAsync(List<Guid> list , string companyId)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(_config.Apis.ElectronicCommerceApi)
        };
        
        client.DefaultRequestHeaders.Add("company", companyId);

        var content = new { id = list };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_config.Apis.ElectronicCommerceApi + "/api/Product/list"),
            Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        var resp = await client.SendAsync(request).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<List<ProductDto>>(await resp.Content.ReadAsStringAsync());
    }
}