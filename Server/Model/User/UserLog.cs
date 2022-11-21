using Server.Interface;

namespace Server.Model.User;

public class UserLog:IUserData
{
    public UInt32 UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastAccess { get; set; }

    public UserLog()
    {
        
    }
    public UserLog(UInt32 userId,DateTime createdDate,DateTime lastAccess)
    {
        UserId = userId;
        CreatedDate = createdDate;
        LastAccess = lastAccess;
    }

    public (string, object) InsertQuery()
    {
        var query = "INSERT INTO user_log(UserId,CreatedDate,LastAccess) " +
                    "VALUES(@userId,@createdDate,@lastAccess)";
        var obj = new
        {
            userId =UserId,
            createdDate = CreatedDate,
            lastAccess = LastAccess
        };
        
        return (query,obj);
    }
}