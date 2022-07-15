using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Habr.Presentation.Services.Implementations
{
    public static class JobService
    {
        public static void StartRecurringJob()
        {
            RecurringJobManager recurringJobManager = new RecurringJobManager();
            recurringJobManager.AddOrUpdate("jobId", () => RatePostUpdateAsync(), Cron.Daily);
        }

        public static async Task RatePostUpdateAsync()
        {
            using var context = new DataContext();

            foreach (var post in context.Posts)
            {
                var postRatings = await context.RatingsPosts
                    .Where(r => r.PostId == post.Id)
                    .ToListAsync();

                if (postRatings.Count > 0)
                {
                    var sumRatings = postRatings.Sum(r => r.Value);
                    post.TotalRating = Math.Round(sumRatings / (double)postRatings.Count, 1);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
