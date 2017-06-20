namespace Destiny
{
    public enum LoginResult : int
    {
        Valid = 0,
        Banned = 3,
        InvalidPassword = 4,
        InvalidUsername = 5,
        LoggedIn = 7
    }
}
