﻿using CsgoServerInterface.CsgoServer;
using CsgoServerInterface.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService;

public class CsgoServerService : ICsgoServerService
{
	public CsgoServerService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	private HttpClient _httpClient;

	public AbstractCsgoServer SelectedServerforNavigation { get; set; }

	public async Task<List<DatHostServer>> GetDatHostServers()
	{
		string uri = _httpClient.BaseAddress.ToString() + "/api/0.1/game-servers";

		using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

		if (responseMessage.IsSuccessStatusCode)
		{
			List<DatHostServer> datHosts = await responseMessage.Content.ReadAsAsync<List<DatHostServer>>();

			for (int i = 0; i < datHosts.Count; i++)
			{
				if (datHosts[i].CsgoSettings == null)
					datHosts.RemoveAt(i);
			}

			return datHosts;
		}
		else
		{
			throw new HttpRequestException(responseMessage.ReasonPhrase, null, responseMessage.StatusCode);
		}
	}
}