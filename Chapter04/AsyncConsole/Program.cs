HttpClient client = new();

HttpResponseMessage response = await client.GetAsync("http://www.officevault.co.za");
WriteLine("Officevault homepage has {0:N0} bytes.", response.Content.Headers.ContentLength);