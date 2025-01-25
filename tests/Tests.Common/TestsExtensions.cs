using Newtonsoft.Json;

namespace Tests.Common;

public static class TestsExtensions
{
    public static async Task<T> ToResponseModel<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content)
               ?? throw new ArgumentException("Response content cannot be null.");
    }

    public static async Task<string> ToResponseModel(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync()
               ?? throw new ArgumentException("Response content cannot be null.");
    }
}