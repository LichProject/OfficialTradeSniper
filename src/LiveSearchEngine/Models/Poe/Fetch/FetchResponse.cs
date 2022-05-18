using System;

namespace LiveSearchEngine.Models.Poe.Fetch
{
    public class FetchResponse
    {
        public Result[] Result { get; set; } = Array.Empty<Result>();
    }
}