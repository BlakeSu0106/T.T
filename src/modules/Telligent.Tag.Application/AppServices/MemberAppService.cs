using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telligent.Core.Infrastructure.Services;
using Telligent.Tag.Application.Configs;
using Telligent.Tag.Application.Dtos.Members;

namespace Telligent.Tag.Application.AppServices;

public class MemberAppService : IAppService
{
    private readonly Config _config;

    public MemberAppService(IOptions<Config> config)
    {
        _config = config.Value;
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid id)
    {
        using var client = new HttpClient();

        var resp = await client.GetAsync(new Uri(_config.Apis.MemberApi + $"/api/Company?id={id}"));
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<CompanyDto>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<MemberDto> GetMemberAsync(Guid id)
    {
        using var client = new HttpClient();
        
        var resp = await client.GetAsync(new Uri(_config.Apis.MemberApi + $"/api/Member?id={id}"));
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<MemberDto>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<IList<MemberDto>> GetMemberListAsync(List<Guid> ids)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(_config.Apis.MemberApi)
        };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_config.Apis.MemberApi + "/api/Member/list"),
            Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        var resp = await client.SendAsync(request).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<IList<MemberDto>>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<ChannelDto> GetChannelAsync(Guid id)
    {
        using var client = new HttpClient();

        var resp = await client.GetAsync(new Uri(_config.Apis.MemberApi + $"/api/Channel?id={id}"));
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<ChannelDto>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<ProspectDto> GetProspectAsync(Guid id)
    {
        using var client = new HttpClient();

        var resp = await client.GetAsync(new Uri(_config.Apis.MemberApi + $"/api/Prospect?id={id}"));
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<ProspectDto>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<IList<ProspectDto>> GetProspectListAsync(List<Guid> ids)
    {
        using var client = new HttpClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_config.Apis.MemberApi + "/api/Prospect/list"),
            Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        var resp = await client.SendAsync(request).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<IList<ProspectDto>>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<CompanyMappingDto> GetCompanyMappingAsync(string id)
    {
        using var client = new HttpClient();

        var resp = await client.GetAsync(new Uri(_config.Apis.MemberApi + $"/api/CompanyMapping?id={id}"));
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<CompanyMappingDto>(await resp.Content.ReadAsStringAsync());
    }

    public async Task<UserDto> GetUserAsync(string id)
    {
        using var client = new HttpClient();

        var resp = await client.GetAsync(new Uri(_config.Apis.MemberApi + $"/api/User?id={id}"));
        resp.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<UserDto>(await resp.Content.ReadAsStringAsync());
    }
}