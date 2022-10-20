namespace Server.Interface;

public interface IUserData
{
   
   public Task<bool> SaveDataToDB();
   public Task<bool> SaveDataToRedis();
}