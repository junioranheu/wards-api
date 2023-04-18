using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wards.Application.AutoMapper;
using Wards.Infrastructure.Data;

namespace Wards.UnitTests.Fixtures
{
    public static class Fixture
    {
        public static WardsContext CriarContext()
        {
            DbContextOptions<WardsContext> mock = new DbContextOptionsBuilder<WardsContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            WardsContext? context = new(mock);

            return context;
        }

        public static IMapper CriarMapper()
        {
            MapperConfiguration mockMapper = new(x =>
            {
                x.AddProfile(new AutoMapperConfig());
            });

            IMapper map = mockMapper.CreateMapper();

            return map;
        }
    }
}