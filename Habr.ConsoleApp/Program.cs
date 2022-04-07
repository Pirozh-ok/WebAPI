using Habr.DataAccess;

using (var context = new DataContext())
{
    Console.WriteLine(context.Posts.Count());
}
