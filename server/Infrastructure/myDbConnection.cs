using LinqToDB;
using LinqToDB.Data;

namespace Infrastructure;

public class myDbConnection : DataConnection
{
    public myDbConnection(DataOptions<myDbConnection> options) : base(options.Options)
    {
        
    }
    public ITable<User> Users => this.GetTable<User>();
    
}











