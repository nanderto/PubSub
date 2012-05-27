using Entities;
using System.Collections.Generic;
public class Users
{
    public Users()
    {
        _usrList.Add(new User
        {
            UserName = "BenM",
            FirstName = "Ben",
            LastName = "Miller",
            City = "Seattle"
        });
        _usrList.Add(new User
        {
            UserName = "AnnB",
            FirstName = "Ann",
            LastName = "Beebe",
            City = "Boston"
        });
    }

    public List<User> _usrList = new List<User>();
    public void Update(User umToUpdate)
    {
        foreach (User um in _usrList)
        {
            if (um.UserName == umToUpdate.UserName)
            {
                _usrList.Remove(um);
                _usrList.Add(umToUpdate);
                break;
            }
        }
    }

    public void Create(User umToUpdate)
    {
        foreach (User um in _usrList)
        {
            if (um.UserName == umToUpdate.UserName)
            {
                throw new System.InvalidOperationException("Duplicat username: " + um.UserName);
            }
        }
        _usrList.Add(umToUpdate);
    }
    public void Remove(string usrName)
    {
        foreach (User um in _usrList)
        {
            if (um.UserName == usrName)
            {
                _usrList.Remove(um);
                break;
            }
        }
    }

    public User GetUser(string uid)
    {
        User usrMdl = null;
        foreach (User um in _usrList)
            if (um.UserName == uid)
                usrMdl = um;
        return usrMdl;
    }
}