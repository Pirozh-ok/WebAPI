using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Habr.Presentation.Services.Implementations
{
    public class JobService
    {
        public static void StartRecurringJob()
        {
            IRecurringJobManager recurringJobManager = new RecurringJobManager();
            recurringJobManager.AddOrUpdate("jobId", () => RatePostUpdateAsync(), Cron.Minutely);
        }

        public static async Task RatePostUpdateAsync()
        {
            using var context = new DataContext();

            foreach (var post in context.Posts)
            {
                var postRatings = await context.RatingsPosts
                    .Where(r => r.PostId == post.Id)
                    .ToListAsync();

                //var sumRatings = postRatings.Aggregate((r1,r2) => r1.Value + r2.Value);
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
