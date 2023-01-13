using Server.Interface;

namespace Server.Model.User;

public class UserAttendance
{
    public UInt32 UserId { get; set; }
    public string ContentType{ get; set; }
    public UInt32 CheckDay { get; set; }

    public Boolean IsChecked { get; set; }

    public UserAttendance(UInt32 userId,string contentType)
    {
        UserId = userId;
        CheckDay = 1;
        ContentType = contentType;

    }

    public UserAttendance()
    {
        
    }
    
    
    public (String,Object) InsertQuery()
    {
        var query = "INSERT INTO user_attendance(UserId,ContentType,CheckDay,IsChecked) " +
                    "VALUES(@userId,@contentType,@checkDay,@isChecked)";
        var obj =new
        {
            userId=UserId,
            contentType=ContentType,
            checkDay= CheckDay,
            isChecked=IsChecked
        };
        return (query,obj);
    }
    
    public (String,Object) UpdateQuery()
    {
        
        var query = "UPDATE user_attendance SET " +
                    "CheckDay=@checkDay," +
                    "IsChecked=@isChecked "+
                    "WHERE UserId=@userId AND ContentType=@contentType";
        var obj=new
        {
            userId =UserId,
            contentType=ContentType,
            checkDay= CheckDay,
            isChecked=IsChecked
        };
                
          

        return (query,obj);
    }
}