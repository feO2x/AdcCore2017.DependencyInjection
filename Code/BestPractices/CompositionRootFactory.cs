using System.Collections.Generic;
using System.Threading.Tasks;
using Light.GuardClauses;
using LightInject;
using Microsoft.EntityFrameworkCore;

namespace BestPractices
{
    public static class CompositionRootFactory
    {
        public static ServiceContainer CreateWithInMemoryEf(string databaseName = "Foo")
        {
            var container = new ServiceContainer();

            var contextOptions =
                new DbContextOptionsBuilder<BloggingContext>()
                    .UseInMemoryDatabase(databaseName)
                    .Options;

            container.RegisterInstance<DbContextOptions>(contextOptions)
                     .Register<BloggingContext>(new PerScopeLifetime());

            return container;
        }

        public static async Task<BloggingContext> AddDefaultBlogsAndPosts(this BloggingContext context)
        {
            context.MustNotBeNull(nameof(context));

            context.Blogs.AddRange(
                                   new Blog
                                   {
                                       Url = "http://blog.ploeh.dk/",
                                       Posts = new List<Post>
                                               {
                                                   new Post { Title = "Test Data Builders in C#", Content = "A brief recap of the Test Data Builder design pattern with examples in C#." },
                                                   new Post { Title = "Generalised Test Data Builder", Content = "This article presents a generalised Test Data Builder." },
                                                   new Post { Title = "The Builder functor", Content = "The Test Data Builder design pattern as a functor." },
                                                   new Post { Title = "Builder as Identity", Content = "In which the Builder functor turns out to be nothing but the Identity functor in disguise." },
                                                   new Post { Title = "Test data without Builders", Content = "We don't need no steenkin' Test Data Builders!" }
                                               }
                                   },
                                   new Blog
                                   {
                                       Url = "http://blog.cleancoder.com/",
                                       Posts = new List<Post>
                                               {
                                                   new Post{Title = "Women in Tech", Content = "I started my career as a programmer in 1969 at a company called A.S.C. Tabulating in Lake Bluff, Illinois."},
                                                   new Post{Title = "Just Following Orders", Content = "The year is 2006. Executives at VW know that their diesel engine can not meet American emissions standards. So they ask the enginers for a solution that does not require a redesign of the engine."}
                                               }
                                   });

            await context.SaveChangesAsync();

            return context;
        }
    }
}