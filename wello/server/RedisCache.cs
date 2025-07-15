using StackExchange.Redis;

namespace server;

public class RedisCache
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;

    public RedisCache(IConfiguration config)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(config["REDIS_URL"] ?? throw new Exception("Need to have a redis url in config"));
        _database = _connectionMultiplexer.GetDatabase();
    }

    public string? GetUser(string username)
    {
        return _database.StringGet(username);
    }

    public void SetUser(string username, string password)
    {
        if (!LockUser(username, password, TimeSpan.FromSeconds(10)))
        {
            Console.WriteLine($"couldn't lock username {username}");
            return;
        }

        _database.StringSet(username, password);
        ReleaseUser(username, password);
    }

    public void DeleteUser(string username)
    {
        var password = GetUser(username) ?? throw new Exception("user doesn't exist");
        if (!LockUser(username, password, TimeSpan.FromSeconds(10)))
        {
            Console.WriteLine($"couldn't lock username {username}");
            return;
        }

        _database.KeyDelete(username);
        ReleaseUser(username, password);
    }

    public bool LockUser(string username, string password, TimeSpan expiry)
    {
        return _database.LockTake($"{username}-lock", password, expiry);
    }

    public bool ReleaseUser(string username, string password)
    {
        return _database.LockRelease($"{username}-lock", password);
    }
}

