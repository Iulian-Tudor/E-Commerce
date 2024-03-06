using Commerce.Business;

namespace Commerce.Infrastructure;

internal sealed class JwtTokenRepository(EfDbContext context) : IJwtTokenRepository
{
    public IQueryable<RefreshTokenReadModel> Query()
    {
        return context.RefreshTokens.AsQueryable();
    }

    public async Task Insert(RefreshTokenReadModel refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task Update(RefreshTokenReadModel refreshToken)
    {
        var existingToken = await context.RefreshTokens.FindAsync(refreshToken.Id) ?? throw new InvalidOperationException("Refresh token not found.");

        context.Entry(existingToken).CurrentValues.SetValues(refreshToken);
        await context.SaveChangesAsync();
    }
}