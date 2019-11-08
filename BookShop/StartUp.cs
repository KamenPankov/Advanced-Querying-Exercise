using BookShop.Data;
using BookShop.Initializer;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BookShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (BookShopContext db = new BookShopContext())
            {
                DbInitializer.ResetDatabase(db);

                Console.WriteLine(GetBooksByAgeRestriction(db, Console.ReadLine()));
                //IncreasePrices(db);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {            
            StringBuilder stringBuilder = new StringBuilder();
          
            IQueryable<string> bookTitles = context.Books
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(b => b);

            foreach (string item in bookTitles)
            {
                stringBuilder.AppendLine(item);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var goldenEditionBooks = context.Books
                 .Where(b => b.EditionType.ToString() == "Gold" && b.Copies < 5000)
                 .OrderBy(b => b.BookId)
                 .Select(b => b.Title);

            foreach (var item in goldenEditionBooks)
            {
                stringBuilder.AppendLine(item);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var booksByPrice = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                });

            foreach (var item in booksByPrice)
            {
                stringBuilder.AppendLine($"{item.Title} - ${item.Price:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var booksNotReleasedIn = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            foreach (var item in booksNotReleasedIn)
            {
                stringBuilder.AppendLine(item);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string[] categories = input.Split();

            for (int i = 0; i < categories.Length; i++)
            {
                categories[i] = categories[i].Trim().ToLower();
            }

            var booksByCategory = context.BookCategories
                .Where(bc => categories.Contains(bc.Category.Name.ToLower()))
                .Select(bc => bc.Book.Title)
                .OrderBy(b => b);

            foreach (var item in booksByCategory)
            {
                stringBuilder.AppendLine(item);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder stringBuilder = new StringBuilder();

            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var booksBeforeDate = context.Books
                .Where(b => b.ReleaseDate.Value < dateTime)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                });

            foreach (var item in booksBeforeDate)
            {
                stringBuilder.AppendLine($"{item.Title} - {item.EditionType} - ${item.Price:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var authorNames = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = string.Join(" ", a.FirstName, a.LastName)
                })
                .OrderBy(a => a.FullName);

            foreach (var item in authorNames)
            {
                stringBuilder.AppendLine(item.FullName);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b);

            foreach (var item in bookTitles)
            {
                stringBuilder.AppendLine(item);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var booksByAuthor = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorName = string.Join(" ", b.Author.FirstName, b.Author.LastName)
                });

            foreach (var item in booksByAuthor)
            {
                stringBuilder.AppendLine($"{item.Title} ({item.AuthorName})");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            
            var booksTitleLonger = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Select(b => b)
                .ToArray();

            return booksTitleLonger.Length;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var bookCopiesByAuthor = context.Authors
                .Select(a => new
                {
                    AuthorName = string.Join(" ", a.FirstName, a.LastName),
                    CopiesCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.CopiesCount);

            foreach (var item in bookCopiesByAuthor)
            {
                stringBuilder.AppendLine($"{item.AuthorName} - {item.CopiesCount}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var profitByCategory = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Profit = c.CategoryBooks.Sum(p => p.Book.Price * p.Book.Copies)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.CategoryName);

            foreach (var item in profitByCategory)
            {
                stringBuilder.AppendLine($"{item.CategoryName} ${item.Profit:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var mostRecentBooks = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    RecentBooks = c.CategoryBooks.OrderByDescending(b => b.Book.ReleaseDate)
                                                .Select(b => new
                                                {
                                                    BookTitle = b.Book.Title,
                                                    ReleaseYear = b.Book.ReleaseDate.Value.Year
                                                })
                                                .Take(3)
                })
                .OrderBy(c => c.CategoryName);

            foreach (var item in mostRecentBooks)
            {
                stringBuilder.AppendLine($"--{item.CategoryName}");

                foreach (var book in item.RecentBooks)
                {
                    stringBuilder.AppendLine($"{book.BookTitle} ({book.ReleaseYear})");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var booksReleased = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .Select(b => b);

            foreach (var item in booksReleased)
            {
                item.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToDelete = context.Books
                .Where(b => b.Copies < 4200)
                .Select(b => b);

            int booksCount = booksToDelete.Count();

            context.Books
                .RemoveRange(booksToDelete);

            context.SaveChanges();

            return booksCount;
        }
    }
}
