using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DB;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingRepository> _logger;

    }
}
