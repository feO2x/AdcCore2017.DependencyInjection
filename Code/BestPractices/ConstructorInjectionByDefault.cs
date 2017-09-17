using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Light.GuardClauses;
using LightInject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BestPractices
{
    public sealed class ConstructorInjectionByDefault
    {
        [Fact]
        public async Task ResolveDifferentControllers()
        {
            var container = CompositionRootFactory.CreateWithInMemoryEf();
            container.EnableAnnotatedPropertyInjection();

            container.Register<ConstructorInjectionController>(new PerScopeLifetime())
                     .Register<PropertyInjectionController>(new PerScopeLifetime())
                     .Register(f => new ServiceLocatorClientController(f), new PerScopeLifetime());

            var expectedBlogNames = new[] { "http://blog.ploeh.dk/", "http://blog.cleancoder.com/" };
            using (container.BeginScope())
            {
                await container.GetInstance<BloggingContext>().AddDefaultBlogsAndPosts();

                var first = container.GetInstance<ConstructorInjectionController>();
                var blogNames = await first.Get();
                blogNames.ShouldBeEquivalentTo(expectedBlogNames);

                var second = container.GetInstance<PropertyInjectionController>();
                blogNames = await second.Get();
                blogNames.ShouldBeEquivalentTo(expectedBlogNames);

                var third = container.GetInstance<ServiceLocatorClientController>();
                blogNames = await third.Get();
                blogNames.ShouldBeEquivalentTo(expectedBlogNames);
            }
        }

        public sealed class ConstructorInjectionController : Controller
        {
            private readonly BloggingContext _context;

            public ConstructorInjectionController(BloggingContext context)
            {
                _context = context.MustNotBeNull(nameof(context));
            }

            public async Task<List<string>> Get()
            {
                return await _context.Blogs
                                     .Select(b => b.Url)
                                     .ToListAsync();
            }
        }

        public sealed class PropertyInjectionController : Controller
        {
            [Inject]
            public BloggingContext Context { get; set; }

            public async Task<List<string>> Get()
            {
                return await Context.Blogs
                                    .Select(b => b.Url)
                                    .ToListAsync();
            }
        }

        public sealed class ServiceLocatorClientController : Controller
        {
            private readonly IServiceFactory _container;

            public ServiceLocatorClientController(IServiceFactory container)
            {
                _container = container;
            }

            public async Task<List<string>> Get()
            {
                var context = _container.GetInstance<BloggingContext>();
                return await context.Blogs
                                    .Select(b => b.Url)
                                    .ToListAsync();
            }
        }
    }
}