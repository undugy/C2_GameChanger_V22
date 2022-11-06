using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserTeam:IUserData
{
    public  UInt32 TeamId { get; set; }
    public UInt32 UserId{ get; set; }
    public string? NickName{ get; set; }
    public UInt32 Point{ get; set; }
    public UInt32 Star{ get; set; }
    public UInt32 Ball{ get; set; }
    public UInt32 Exp{ get; set; }
    public Int32 TeamLevel{ get; set; }
    public string Intro{ get; set; }
    
    public override string ToString()
    {
        return "UserTeam";
    }
    
    public (String,Object) InsertQuery()
    {
       
        var query = "INSERT INTO user_team(UserId,NickName,Point,Star,Ball,Exp,TeamLevel,Intro) " +
                         "VALUES(@userId,@nickName,@point,@star,@ball,@exp,@teamLevel,@intro)";
        var obj =new
        {
            userId=UserId,
            nickName=NickName,
            point=Point,
            star=Star,
            ball=Ball,
            exp=Exp,
            teamLevel=TeamLevel,
            intro=Intro
        };
                
        return (query,obj);
    }
    public (String,Object) UpdateQuery()
    {
        var query = "UPDATE user_team " +
                         "SET " +
                         "UserId=@userId," +
                         "NickName=@nickName," +
                         "Point=@point, " +
                         "Star=@star, " +
                         "Ball=@ball, " +
                         "Exp=@exp, " +
                         "TeamLevel=@teamLevel, " +
                         "Intro=@intro " +
                         "WHERE UserId=@userId"; 
        var obj = new
                {
                    userId=UserId,
                    nickName=NickName,
                    point=Point,
                    star=Star,
                    ball=Ball,
                    exp=Exp,
                    teamLevel=TeamLevel,
                    intro=Intro
                };

        return (query, obj);
    }
    
    
    // public static async Task<UserTeam> SelectQueryOrDefaultAsync(string userID)
    // {
    //     UserTeam? userInfo=null;
    //     try
    //     {
    //         using (var conn = await DBManager.GetDBConnection())
    //         {
    //             userInfo=await conn.QuerySingleOrDefaultAsync<UserTeam>("SELECT * FROM user_team WHERE userId=@ID",
    //                 new { ID = userID });
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return userInfo;
    //     }
    //
    //     return userInfo;
    // }
}