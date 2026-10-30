using Facet;
using Infrastructure;

namespace Service.Dtos;

[Facet(
    sourceType: typeof(User),
    exclude:
    [
        nameof(User.PasswordHash),
        nameof(User.Salt)
    ]
)]  
public partial class UserDto;