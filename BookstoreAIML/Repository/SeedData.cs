using Microsoft.EntityFrameworkCore;

namespace BookstoreAIML.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if(!_context.Products.Any())
            {

            }
        }
    }
}
