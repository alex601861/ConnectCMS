using CMSTrain.Application.Common.Service;

namespace CMSTrain.Identity.Implementation.Manager;

public class TokenManager : ISingletonService
{
    public readonly HashSet<string> BlackList = new();
}