using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dev.Lau.WebApi.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext(options)
{
}