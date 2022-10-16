namespace Server;

public enum ErrorCode:Int32
{
    NONE=0,
    NOID,
    WRONG_PW,
    
    ALREADY_EXIST=10,
    CREATE_FAIL
}