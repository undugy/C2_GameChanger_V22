using Dapper;
using Server.Interface;
using Server.Services;

namespace Server.Model.User;

public class UserTeam:IUserData
{
    public UInt32 UserId{ get; set; }
    public  UInt32 TeamId { get; set; }
    public string? NickName{ get; set; }
    public UInt32 Point{ get; set; }
    public UInt32 Star{ get; set; }
    public UInt32 Ball{ get; set; }
    public UInt32 Exp{ get; set; }
    public UInt32 TeamLevel{ get; set; }
    public string Intro{ get; set; }

    public UserTeam()
    {
        
    }

    public UserTeam(UInt32 teamId,UInt32 userId, string?nickName)
    {
        TeamId = teamId;
        UserId = userId;
        NickName = nickName;
    }
    
    public override string ToString()
    {
        return "UserTeam";
    }
    
    public (String,Object) InsertQuery()
    {
       
        var query = "INSERT IGNORE INTO user_team(TeamId,UserId,NickName) " +
                         "VALUES(@teamId,@userId,@nickName)";
        var obj =new
        {
            teamId=TeamId,
            userId=UserId,
            nickName=NickName
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
    
    
    public (String,Object) UpdateWealthQuery(string wealthName,UInt32 quantity)
    {
        var query = "UPDATE user_team SET "+wealthName+"="+wealthName+"+@Quantity WHERE UserId=@userId";
        
        var obj=new
        {
            userId =UserId,
            Quantity = quantity,
            WealtName = wealthName
        };
        return (query,obj);
    }
}