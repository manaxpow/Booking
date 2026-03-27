public static class CacheExtensions
{
    public static void AddCacheExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = CacheKeyFactory.AppPrefix;
        });


        services.AddStackExchangeRedisOutputCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Out_";
            // options.InstanceName = CacheKeyFactory.OutputCachePrefix;
        });

        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(policy => policy.NoCache());

            options.AddPolicy("DestinationSearchResult", builder =>
            {

                builder.Expire(TimeSpan.FromMinutes(10))
                    .SetVaryByQuery("page", "pageSize", "province", "sortBy", "sortDescending")
                    .Tag(CacheKeyFactory.DestinationTag);

                builder.AddPolicy<PublicCachePolicy>();
            });

            options.AddPolicy("CarSearchResult", builder =>
            {

                builder.Expire(TimeSpan.FromMinutes(10))
                    .SetVaryByQuery("page", "pageSize", "keyword", "brand")
                    .Tag(CacheKeyFactory.CarTag)
                    .SetCacheKeyPrefix("catalog:car:list:");

                builder.AddPolicy<PublicCachePolicy>();
            });

        });
    }
}