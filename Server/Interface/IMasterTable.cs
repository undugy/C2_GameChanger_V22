namespace Server.Interface;

public interface IMasterTable
{
    public bool Load();
    public string GetRedisKey();
}