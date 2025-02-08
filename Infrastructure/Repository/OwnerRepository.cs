

using Domain.Entities;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class OwnerRepository:Repository<Owner>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OwnerRepository> _logger;

        public OwnerRepository(ApplicationDbContext context, ILogger<OwnerRepository> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
       
    }
}
