using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using DragonAPI.Configurations;
// using TrueSight.Lite.Common;
using DragonAPI.Helpers;

namespace DragonAPI.Repositories
{
    public interface ICacheRepository
    {
        public IDatabase GetRedisDb();
        Task<T> GetFromCache<T>(string key);
        Task SetToCache<T>(string key, T value, long expiredSecond = 1800);
        Task SetToCache<T>(string key, T value, TimeSpan expired);
        Task SetToCache<T>(string key, T value, DateTime expiredAt);
        Task RemoveFromCache(string key);
        // Task RemoveKeyPattern(string keyPattern);
    }

    public class CacheRepository : ICacheRepository
    {
        private IRedisStore RedisStore;
        private const string DefaultPrefix = "rongosservice";
        public CacheRepository(IRedisStore RedisStore)
        {
            this.RedisStore = RedisStore;
        }

        public IDatabase GetRedisDb() => RedisStore.GetDatabase();

        public async Task<T> GetFromCache<T>(string key)
        {
            try
            {
                byte[] value = await GetRedisDb().StringGetAsync(key);
                if (value == null)
                    return default;
                T cachedResult = Deserialize<T>(value);
                return cachedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetFromCache:" + key + ex.Message);
                return default;
            }
        }

        public async Task SetToCache<T>(string key, T value, long expiredSecond = 1800)
        {
            await SetToCache(key, value, TimeSpan.FromSeconds(expiredSecond));
        }

        public async Task SetToCache<T>(string key, T value, TimeSpan expired)
        {
            try
            {
                byte[] json = Serialize(value);
                IDatabase Database = GetRedisDb();
                await Database.StringSetAsync(key, json, expired);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetToCache:" + key + ex.Message);
            }
        }
        public async Task SetToCache<T>(string key, T value, DateTime expiredAt)
        {
            try
            {
                byte[] json = Serialize(value);
                IDatabase Database = GetRedisDb();
                await Database.StringSetAsync(key, json, expiredAt.Subtract(DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetToCache:" + key + ex.Message);
            }
        }

        public async Task RemoveFromCache(string key)
        {
            try
            {
                try
                {
                    IDatabase Database = GetRedisDb();
                    await Database.KeyDeleteAsync(key, CommandFlags.FireAndForget);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RemoveFromCache:" + key + ex.Message);
                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine("RemoveFromCache:" + ex2.Message);
            }
        }

        private byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value);
        }
        private T Deserialize<T>(byte[] array)
        {
            return JsonSerializer.Deserialize<T>(array);
        }

        // public async Task RemoveKeyPattern(string keyPattern)
        // {
        //     try
        //     {
        //         IServer Server = RedisStore.GetServer();
        //         var RedisKeys = Server.Keys(0, $"{keyPattern}*");
        //         foreach (var k in RedisKeys)
        //         {
        //             try
        //             {
        //                 IDatabase Database = RedisStore.GetDatabase();
        //                 await Database.KeyDeleteAsync(k, CommandFlags.FireAndForget);
        //             }
        //             catch (Exception ex)
        //             {
        //                 Console.WriteLine("RemoveFromCache:" + keyPattern + ex.Message);
        //             }
        //         }
        //     }
        //     catch (Exception ex2)
        //     {
        //         Console.WriteLine("RemoveFromCache:" + ex2.Message);
        //     }
        // }
    }
}
