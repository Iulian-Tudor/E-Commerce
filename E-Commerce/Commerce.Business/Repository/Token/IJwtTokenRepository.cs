namespace Commerce.Business;

public interface IJwtTokenRepository
{
    IQueryable<RefreshTokenReadModel> Query();

    Task Insert(RefreshTokenReadModel refreshToken);

    Task Update(RefreshTokenReadModel refreshToken);
}