using Habr.BusinessLogic.Services.Implementations;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

class Program
{
    static private IUserService userService = new UserService();
    static private IPostService postService = new PostService();
    static private ICommentService commentService = new CommentService();

    static private void ReadPostAllUser()
    {
        var posts = postService.GetPosts();

        if (posts == null || posts.Count() == 0)
        {
            Console.WriteLine("Нет опубликованных постов на сервисе!");
            return;
        }

        foreach (var post in posts)
        {
            Console.WriteLine($"Автор поста: {post.User.Name}");
            Console.WriteLine($"Название поста: {post.Title}");
            Console.WriteLine($"Дата создания: {post.Created}");
            Console.WriteLine($"Текст: {post.Text}\n");
        }
    }

    static private void ReadAllOwnPosts(User user)
    {
        var publishedPosts = postService.GetPublishedPostsByUser(user.Id);
        var notPublishedPosts = postService.GetNotPublishedPostsByUser(user.Id);

        if (publishedPosts == null || publishedPosts.Count() == 0)
        {
            Console.WriteLine("У вас ещё нет опубликованных постов");
        }
        else
        {
            Console.WriteLine("Опубликованные посты: ");
            foreach (var post in publishedPosts)
            {
                Console.WriteLine($"Название поста: {post.Title}");
                Console.WriteLine($"Дата создания: {post.Created}");
                Console.WriteLine($"Текст: {post.Text}\n");
            }
        }

        if (notPublishedPosts == null || notPublishedPosts.Count() == 0)
        {
            Console.WriteLine("У вас нет постов в черновиках");
        }
        else
        {
            Console.WriteLine("Посты в черновиках: ");
            foreach (var post in notPublishedPosts)
            {
                Console.WriteLine($"Название поста: {post.Title}");
                Console.WriteLine($"Дата создания: {post.Created}");
                Console.WriteLine($"Текст: {post.Text}\n");
            }
        }
    }

    static private void CreateNewPost(User user)
    {
        Console.Write("Введите название поста: ");
        var title = Console.ReadLine();
        Console.Write("Введите текст поста: ");
        var text = Console.ReadLine();
        Console.Write("Опубликовать пост? (да/нет): ");
        bool isPublished = Console.ReadLine() == "да";
        try
        {
            postService.CreatePost(title, text, user.Id, isPublished);

            if (isPublished)
            {
                Console.WriteLine("Пост успешно опубликован!");
            }
            else
            {
                Console.WriteLine("Пост сохранён в черновиках!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Не удалось опубликовать пост!");
        }
    }

    static private void CreateCommentPost(User user)
    {
        Console.WriteLine("Выберете пост для комментирования: ");
        var posts = postService.GetPublishedPosts();

        if (posts == null || posts.Count() == 0)
        {
            Console.WriteLine("Нет постов для комментирования");
            return;
        }

        for (int i = 0; i < posts.Count(); i++)
        {
            Console.WriteLine($"{i + 1} - {posts.ElementAt(i).Title}");
        }

        Console.Write("Введите номер поста для комментирования: ");

        try
        {
            var numberPost = int.Parse(Console.ReadLine()) - 1;

            if (numberPost < 0 || numberPost > posts.Count())
            {
                throw new Exception();
            }

            var post = posts.ElementAt(numberPost);
            var commentsPost = commentService.GetCommentsByPost(post.Id);

            foreach (var comment in commentsPost)
            {
                Console.WriteLine($"{comment.User.Name}: {comment.Text}");
                foreach (var subcomment in comment.SubComments)
                    Console.WriteLine($"\t{subcomment.User.Name}: {subcomment.Text}");
            }

            Console.Write("Введите текст комментария: ");
            var commentText = Console.ReadLine();

            commentService.CreateComment(user.Id, post.Id, commentText);
            Console.WriteLine("Комментарий к посту добавлен!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Поста с таким номером не существует!");
            return;
        }
    }

    static private void DeleteComment(User user)
    {
        Console.WriteLine("Выберете комментарий для удаления: ");
        var comments = commentService.GetCommentsByUser(user.Id);

        if (comments == null || comments.Count() == 0)
        {
            Console.WriteLine("Нет комментариев для удаления");
            return;
        }

        for (int i = 0; i < comments.Count(); i++)
        {
            Console.WriteLine($"{i + 1} - {comments.ElementAt(i).Text}");
        }

        Console.Write("Введите номер поста для комментирования: ");
        try
        {
            var numberComment = int.Parse(Console.ReadLine()) - 1;

            if (numberComment < 0 || numberComment > comments.Count())
            {
                throw new Exception();
            }

            commentService.DeleteComment(comments.ElementAt(numberComment).Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Комментария с таким номером не существует!");
            return;
        }
    }

    static private void AnswerComment(User user)
    {
        Console.WriteLine("Выберете комментарий для ответа: ");
        var comments = commentService.GetComments();

        if (comments == null || comments.Count() == 0)
        {
            Console.WriteLine("Нет комментариев для ответа");
            return;
        }

        for (int i = 0; i < comments.Count(); i++)
        {
            Console.WriteLine($"{i + 1} - {comments.ElementAt(i).Text}");
        }

        Console.Write("Введите номер комментария для ответа: ");

        try
        {
            var numberComment = int.Parse(Console.ReadLine()) - 1;

            if (numberComment < 0 || numberComment > comments.Count())
            {
                throw new Exception();
            }

            Console.Write("Введите текст комментария: ");
            var textComment = Console.ReadLine();
            commentService.CreateCommentAnswer(user.Id, textComment, comments.ElementAt(numberComment).Id, comments.ElementAt(numberComment).PostId);
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Комментария с таким номером не существует!");
            return;
        }
    }

    static private void PublishPost(User user)
    {
        Console.WriteLine("Выберете пост для публикации: ");
        var posts = postService.GetNotPublishedPostsByUser(user.Id);

        if (posts == null || posts.Count() == 0)
        {
            Console.WriteLine("В черновиках нет постов для публикации");
            return;
        }

        for (int i = 0; i < posts.Count(); i++)
        {
            Console.WriteLine($"{i + 1} - {posts.ElementAt(i).Title}");
        }

        Console.Write("Введите номер поста для публикации: ");
        try
        {
            var numberPost = int.Parse(Console.ReadLine()) - 1;

            if (numberPost < 0 || numberPost > posts.Count())
            {
                throw new Exception("Поста с таким номером не существует!");
            }

            postService.PublishPost(posts.ElementAt(numberPost).Id);
            Console.WriteLine("Пост опубликован!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }

    static private void SendPostToDrafts(User user)
    {
        Console.WriteLine("Выберете пост для переноса в черновик: ");
        var posts = postService.GetPublishedPostsByUser(user.Id);

        if (posts == null || posts.Count() == 0)
        {
            Console.WriteLine("У вас нет опубликованных постов!");
            return;
        }

        for (int i = 0; i < posts.Count(); i++)
        {
            Console.WriteLine($"{i + 1} - {posts.ElementAt(i).Title}");
        }

        Console.Write("Введите номер поста для перемещения в черновики: ");
        try
        {
            var numberPost = int.Parse(Console.ReadLine()) - 1;

            if (numberPost < 0 || numberPost > posts.Count())
            {
                throw new Exception("Поста с таким номером не существует!");
            }

            postService.SendPostToDrafts(posts.ElementAt(numberPost).Id);
            Console.WriteLine("Пост перенесён в черновик!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }

    static private void ChoiseActionInAccount(User user)
    {
        while (true)
        {
            Console.WriteLine(
                "Просмотреть свои посты - 1, " +
                "создать пост - 2, " +
                "удалить пост - 3, " +
                "изменить пост - 4, " +
                "просмотреть все посты - 5, " +
                "написать комментарий к посту - 6, " +
                "удалить комментарий - 7, " +
                "ответить на комментарий - 8, " +
                "опубликовать пост из черновиков - 9, " +
                "отправить опубликованный пост в черновики - 10, " +
                "выход - 11"
                );

            switch (Console.ReadLine())
            {
                case "1":
                    ReadAllOwnPosts(user);
                    break;
                case "2":
                    CreateNewPost(user);
                    break;
                case "3":
                    DeletePost(user);
                    break;
                case "4":
                    UpdatePost(user);
                    break;
                case "5":
                    ReadPostAllUser();
                    break;
                case "6":
                    CreateCommentPost(user);
                    break;
                case "7":
                    DeleteComment(user);
                    break;
                case "8":
                    AnswerComment(user);
                    break;
                case "9":
                    PublishPost(user);
                    break;
                case "10":
                    SendPostToDrafts(user);
                    break;
                case "11":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный номер операции...");
                    break;
            }
        }
    }

    static private Post PrintAllOwnPostForChoice(User user, string message)
    {
        var posts = postService.GetPostsByUser(user.Id);
        if (posts == null || posts.Count() == 0)
        {
            Console.WriteLine("У вас ещё нет опубликованных постов");
            return null;
        }

        for (int i = 0; i < posts.Count(); i++)
            Console.WriteLine($"{i + 1} - {posts.ElementAt(i).Title}");

        Console.Write(message);
        try
        {
            int numberPost = int.Parse(Console.ReadLine()) - 1;
            if (numberPost < 0 || numberPost > posts.Count())
                throw new Exception("Поста с таким номером не существует!");

            return posts.ElementAt(numberPost);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    static private void DeletePost(User user)
    {
        var deletePost = PrintAllOwnPostForChoice(user, "\nВведите номер поста, который хотите удалить: ");

        if (deletePost == null) return;

        try
        {
            postService.DeletePost(deletePost.Id);
            Console.WriteLine("Пост успешно удалён!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Не удалось удалить выбраный пост!");
        }
    }

    static private void UpdatePost(User user)
    {
        var updatePost = PrintAllOwnPostForChoice(user, "\nВведите номер поста, который хотите редактировать: ");

        if (updatePost == null)
        {
            return;
        }

        bool isNotExit = true;

        while (isNotExit)
        {
            Console.WriteLine("Изменить название поста - 1, изменить текст поста - 2, выйти из режима редактирования - 3");
            switch (Console.ReadLine())
            {
                case "1":
                    {
                        Console.Write("Введите новое название поста: ");
                        updatePost.Title = Console.ReadLine();
                        break;
                    }
                case "2":
                    {
                        Console.Write("Введите новый текст поста: ");
                        updatePost.Text = Console.ReadLine();
                        break;
                    }
                case "3":
                    {
                        isNotExit = false;
                        return;
                    }
                default:
                    Console.WriteLine("Неверный номер операции!");
                    break;
            }

            try
            {
                postService.UpdatePost(updatePost);
                Console.WriteLine("Изменения в посте сохранены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static private void LogIn()
    {
        Console.WriteLine("Регистрация...");
        Console.Write("Введите имя: ");
        var name = Console.ReadLine();

        Console.Write("Введите адрес электронной почты: ");
        var email = Console.ReadLine();

        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();

        try
        {
            userService.Register(name, email, password);
            Console.WriteLine("Регистрация прошла успешно!");
            SignIn();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static private void SignIn()
    {
        Console.WriteLine("Вход...");
        Console.Write("Введите адрес электронной почты: ");
        var email = Console.ReadLine();

        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();

        try
        {
            var currentUser = userService.LogIn(email, password);
            Console.WriteLine("Вход выполнен: ");
            ChoiseActionInAccount(currentUser);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Войти - 1, зарегистрироваться - 2");
            switch (Console.ReadLine())
            {
                case "1":
                    SignIn();
                    break;
                case "2":
                    LogIn();
                    break;
                default:
                    Console.WriteLine("Неверный номер операции...");
                    break;
            }
        }
    }
}
