using Facet;
using Infrastructure;

namespace Service.Dtos;

[Facet(
    sourceType: typeof(User),
    exclude:
    [
        nameof(User.PasswordHash),
    ]
)]  
public partial class UserDto;