using Dapper;
using Twitter_new.Models;
using Twitter_new.Utilities;

namespace Twitter_new.Repositories;
public interface IUserRepository
{

    Task<User> GetUserById(long Id);
    Task<User> FindEmail(string email);
    Task<User> UserLogin(string Email, string Passcode);
    Task<User> CreateUser(User item);
    Task<bool> UpdateUser(User item);


}

public class UserRepository : BaseRepository, IUserRepository
{

    public UserRepository(IConfiguration config) : base(config)
    {

    }
    public async Task<User> CreateUser(User item)
    {
        var query = $@"INSERT INTO ""{TableNames.users}""
        (full_name,email,password)
	     VALUES (@FullName,@Email,@Password)
        RETURNING *";

        using (var con = NewConnection)
        {
            var res = await con.QuerySingleOrDefaultAsync<User>(query, item);
            return res;
        }
    }




    public async Task<User> GetUserById(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.users}""
        WHERE id = @Id";
        // SQL-Injection

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Id });

    }

    public async Task<bool> UpdateUser(User item)
    {
        var query = $@"UPDATE ""{TableNames.users}"" SET full_name = @FullName WHERE Id = @Id";

        using (var con = NewConnection)
        {
            var rowCount = await con.ExecuteAsync(query, item);
            return rowCount == 1;
        }


    }



    public async Task<User> UserLogin(string Email, string Passcode)
    {
        var query = $@"SELECT * FROM ""{TableNames.users}""
        WHERE email = @Email AND passcode = @Passcode";
        // SQL-Injection

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Email, Passcode });
    }


    public async Task<User> FindEmail(string email)
    {
        // Query
        var query = $@"SELECT * FROM ""{TableNames.users}"" WHERE email = @Email";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { email });

    }
}

