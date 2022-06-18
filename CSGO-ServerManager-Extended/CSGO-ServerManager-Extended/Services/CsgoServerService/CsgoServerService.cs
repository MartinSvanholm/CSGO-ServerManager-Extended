using CsgoServerInterface.CsgoServer;
using CsgoServerInterface.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService;

public class CsgoServerService
{
	public CsgoServerService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	private HttpClient _httpClient;

	public async Task<List<DatHostServer>> GetDatHostServers()
	{
		string uri = _httpClient.BaseAddress?.ToString() + "/api/0.1/game-servers/";

		using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

		if(responseMessage.IsSuccessStatusCode)
		{
			return await responseMessage.Content.ReadAsAsync<List<DatHostServer>>();
		}
		else
		{
			throw new HttpRequestException(responseMessage.ReasonPhrase, null, responseMessage.StatusCode);
		}
    }
}
