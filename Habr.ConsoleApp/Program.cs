using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

bool AddUser(string name, string email, DateTime registrationDate)
{
    try
    {
        using var context = new DataContext();

        context.Users.Add(new User()
        {
            Name = name,
            Email = email,
            RegistrationDate = registrationDate
        });

        context.SaveChanges();
        return true;
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.Message);
        return false;
    }
}

bool AddPost(string userName, string title, string text, DateTime createDate)
{
    try
    {
        using var context = new DataContext();

        var authorPost = context.Users.Where(u => u.Name == userName).FirstOrDefault();

        if (authorPost != null)
        {
            context.Posts.Add(new Post
            {
                Title = title,
                Text = text,
                Created = createDate,
                User = authorPost
            });

            context.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.Message);
        return false;
    }
}

bool AddComment(string namePost, string text, DateTime createDate, string parentComment, string userName)
{
   try
    {
        using var context = new DataContext();

        var post = context.Posts.FirstOrDefault(p => p.Title == namePost); 
        if (post == null) 
            return false;

        var parentCommentObj = context.Comments.FirstOrDefault(c => c.Text == parentComment);
        var user = context.Users.FirstOrDefault(u => u.Name == userName);
    if (user == null)
            return false;


        context.Comments.Add(new Comment()
        {
            Post = post,
            Text = text,
            CreateDate = createDate,
            Parent = parentCommentObj,
            User = user
        });;

        context.SaveChanges();
        return true;
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.Message);
        return false;
    }    
}

void PrintDataBase()
{
    using var context = new DataContext();
    var users = context.Users
        .Include(u => u.Posts)
        .ThenInclude(p => p.Comments)
        .ThenInclude(c => c.SubComments);

    foreach(var u in users)
    {
        Console.WriteLine($"Имя пользователя: {u.Name}, почта: {u.Email}, дата регистрации: {u.RegistrationDate.ToShortDateString()}\n\tСписок постов:");
        foreach(var p in u.Posts)
        {
            Console.WriteLine($"\tНазвание: {p.Title}, дата создания: {p.Created}\n\t\tСписок комментариев:");
            foreach (var c in p.Comments)
            {
                Console.WriteLine($"\t\tТекст комментария: {c.Text}, дата написания: {c.CreateDate}\n\t\t\tСписок комментариев:");
                foreach (var sc in c.SubComments)
                    Console.WriteLine($"\t\t\tТекст комментария: {sc.Text}, дата написания: {sc.CreateDate}");
            }
        }
    }
}

/*    if (AddUser("Воротников Иван", "some-email@mail.ru", DateTime.Now))
        Console.WriteLine("Пользователь добавлен");
    else Console.WriteLine("Пользователь добавлен");

    if (AddPost("Василий Пупкин", "Введение в EF core", "текст статьи", DateTime.Now))
        Console.WriteLine("Пост добавлен");
    else
        Console.WriteLine("Автора с заданым именем нет! Пост не добавлен");

if (AddComment("Введение в EF core", "Рад, что статья была полезной", DateTime.Now, "Замечательная статья для новичков. Спасибо!", "Василий Пупкин"))
    Console.WriteLine("Комментарий добавлен");
else
    Console.WriteLine("Комментарий не добавлен");*/

PrintDataBase();
