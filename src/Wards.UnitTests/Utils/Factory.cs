using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wards.Application.AutoMapper;
using Wards.Infrastructure.Data;

namespace Wards.UnitTests.Utils
{
    public class Factory
    {
        private readonly WardsContext _context;
        private readonly IMapper _map;

        public Factory()
        {
            var mock = new DbContextOptionsBuilder<WardsContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new WardsContext(mock);

            var mockMapper = new MapperConfiguration(x =>
            {
                x.AddProfile(new AutoMapperConfig());
            });

            _map = mockMapper.CreateMapper();
        }

        public WardsContext CriarContext()
        {
            return _context;
        }

        public IMapper CriarMapper()
        {
            return _map;
        }
    }
}