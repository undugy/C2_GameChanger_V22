namespace Server.Model.User;

public class UserAttendance
{
    public UInt32 UserId { get; set; }
    public string ContentType{ get; set; }
    public UInt32 CheckDay { get; set; }
    public DateTime RecvDate { get; set; }
    public Boolean IsChecked  { get; set; }

    public UserAttendance(UInt32 userId,string contentType,DateTime recvDate)
    {
        UserId = userId;
        ContentType = contentType;
        RecvDate = recvDate;
    }
    
    
    
    
    public (String,Object) InsertQuery()
    {
        var query = "INSERT INTO user_attendance(UserId,ContentType,CheckDay,RecvDate,IsChecked) " +
                    "VALUES(@userId,@contentType,@checkDay,@receiveDate,@isChecked)";
        var obj =new
        {
            userId=UserId,
            contentType=ContentType,
            checkDay= CheckDay,
            receiveDate=RecvDate,
            isChecked=IsChecked
        };
        return (query,obj);
    }
    
    public (String,Object) UpdateQuery()
    {
        
        var query = "UPDATE user_attendance SET " +
                    "CheckDay=@checkDay," +
                    "RecvDate=@receiveDate, "+
                    "IsChecked=@isChecked "+
                    "WHERE UserId=@userId AND ContentType=@contentType";
        var obj=new
        {
            userId =UserId,
            contentType=ContentType,
            checkDay= CheckDay,
            receiveDate=RecvDate,
            isChecked=IsChecked
        };
                
          

        return (query,obj);
    }
}