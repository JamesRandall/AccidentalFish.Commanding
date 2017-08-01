﻿using System;
using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Cache
{
    public interface ICacheWrapper
    {
        Task Set(string key, object value, TimeSpan lifeTime);
        Task Set(string key, object value, DateTime expiresAt);
        Task<T> Get<T>(string key);
    }
}
