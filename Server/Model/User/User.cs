using Server.Interface;

namespace Server.Model.User;

public class User
{
    private Int32 _id;
    private string _passWord;
    private List<IUserData> _userDatas;
    

    public async Task<bool> CreateUser(string id,string pw)
    {
        //TODO 여기서 레디스랑 DB조사
        UserInfo userInfo = new UserInfo();
        return false;
    }

    

}