[System.Serializable]
public class UserData
{
    public string username;
    public string email;
    public string userId;
    public string createdAt;

    public UserData(string username, string email, string userId)
    {
        this.username = username;
        this.email = email;
        this.userId = userId;
    }
}