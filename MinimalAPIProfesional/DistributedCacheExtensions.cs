using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MinimalAPIProfesional
{
     public static class DistributedCacheExtensions
     {
          public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key) where T : class                                // Le where permet de spécifier que T doit être une classe
          {
               var json = await cache.GetStringAsync(key);
               if (string.IsNullOrEmpty(json))
               {
                    return default;
               }
               else
               {
                    return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });           // L'option "PropertyNameCaseInsensitive" précise que la désérialisation doit être insensible à la casse
               }
          }



          public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value) where T: class
          {
               var json = JsonSerializer.Serialize(value);
               await cache.SetStringAsync(key, json);
          }
     }
}
