using Blog.Authorization;
using Blog.Data.Entities;
using Blog.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog.Data
{
    public static class SeedData
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider,
            IConfiguration configuration
            )
        {
            DbContextOptions<ApplicationContext> options =
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>();

            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();


            using (ApplicationContext context = new ApplicationContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                if (context.Posts.Any())
                {
                    return;
                }

                // Add Claims **********************************************************
                // ниже для ясного примера я специально вынес всех юзеров отдельно

                string surerAdminEmail = configuration.GetSection("SuperAdminEmail").Value;
                string superAdminPassword = configuration.GetSection("SuperAdminPassword").Value;

                if (await userManager.FindByNameAsync(surerAdminEmail) == null)
                {
                    User superAdmin = new User
                    {
                        Email = surerAdminEmail,
                        UserName = surerAdminEmail,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(superAdmin, superAdminPassword);
                    if (result.Succeeded)
                    {
                        //Add Claims
                        Claim claim1 = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);
                        Claim claim2 = new Claim(MyClaims.Admin, MyClaims.Admin);
                        Claim claim3 = new Claim(MyClaims.SuperAdmin, MyClaims.SuperAdmin);

                        await userManager.AddClaimAsync(superAdmin, claim1);
                        await userManager.AddClaimAsync(superAdmin, claim2);
                        await userManager.AddClaimAsync(superAdmin, claim3);
                    }
                }

                // just for test users claims we add test users as admin and postsWriter

                string adminEmail = "Admin@gmail.com";
                string adminPassword = "1";

                if (await userManager.FindByNameAsync(adminEmail) == null)
                {
                    User admin = new User
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        //Add Claims
                        Claim claim1 = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);
                        Claim claim2 = new Claim(MyClaims.Admin, MyClaims.Admin);
                        //Claim claim3 = new Claim(MyClaims.SuperAdmin, MyClaims.SuperAdmin);

                        await userManager.AddClaimAsync(admin, claim1);
                        await userManager.AddClaimAsync(admin, claim2);
                        //await userManager.AddClaimAsync(admin, claim3);
                    }
                }

                string postsWriterEmail = "PostsWriter@gmail.com";
                string postsWriterPassword = "1";

                if (await userManager.FindByNameAsync(postsWriterEmail) == null)
                {
                    User postsWriter = new User
                    {
                        Email = postsWriterEmail,
                        UserName = postsWriterEmail,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(postsWriter, postsWriterPassword);
                    if (result.Succeeded)
                    {
                        //Add Claims
                        Claim claim1 = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);
                        //Claim claim2 = new Claim(MyClaims.Admin, MyClaims.Admin);
                        //Claim claim3 = new Claim(MyClaims.SuperAdmin, MyClaims.SuperAdmin);

                        await userManager.AddClaimAsync(postsWriter, claim1);
                        //await userManager.AddClaimAsync(postsWriter, claim2);
                        //await userManager.AddClaimAsync(postsWriter, claim3);
                    }
                }
                // End of Add Claims ***********************************************
                else
                {
                    ILogger logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Cannot find the surerAdminEmail");
                    return;
                }

                string[] pictures = new string[]
                {
                    "/images/ukraine1.jpg",
                    "/images/ukraine2.jpg",
                    "/images/ukraine3.jpg",
                    "/images/ukraine4.png",
                    "/images/ukraine5.jpg",
                    "/images/ukraine6.jpg",
                    "/images/ukraine7.jpg",
                    "/images/ukraine8.jpg",
                    "/images/ukraine9.jpg",
                    "/images/ukraine10.png",

                    "/images/music1.jpg",
                    "/images/music2.jpg",
                    "/images/music3.jpg",
                    "/images/music4.jpg",
                    "/images/music5.jpg",
                    "/images/music6.jpg",
                    "/images/music7.jpg",
                    "/images/music8.jpg",
                    "/images/music9.jpg",
                    "/images/music10.jpg",

                    "/images/astronomy1.jpg",
                    "/images/astronomy2.jpg",
                    "/images/astronomy3.jpg",
                    "/images/astronomy4.jpg",
                    "/images/astronomy5.jpg",
                    "/images/astronomy6.jpg",
                    "/images/astronomy7.jpg",
                    "/images/astronomy8.jpg",
                    "/images/astronomy9.jpg",
                    "/images/astronomy10.jpg",
                    "/images/astronomy11.jpg",
                    "/images/astronomy12.jpg",
                    "/images/astronomy13.jpg",
                    "/images/astronomy14.jpg",
                    "/images/astronomy15.jpg",
                    "/images/astronomy16.jpg",
                    "/images/astronomy17.jpg",
                    "/images/astronomy18.jpg",
                    "/images/astronomy19.jpg",
                    "/images/astronomy20.jpg",
                };
                // ///////////////////////////////////////////////////////////////


                IList<Category> categories = new List<Category>
                {
                    new Category{ Name = "Ukraine" },
                    new Category{ Name = "Music" },
                    new Category{ Name = "Astronomy" },
                };
                // ///////////////////////////////////////////////////////////////


                string superAdminId = userManager.FindByNameAsync(surerAdminEmail).Result.Id;
                IList<Comment> comments = new List<Comment>
                {
                    new Comment
                    {
                        Message = "This is a first parent comment",
                        Created = DateTime.Now,
                        PostId = 1,
                        UserId = superAdminId,
                        ParentCommentId = null,

                        ChildComments = new List<Comment>
                        {
                            new Comment
                            {
                                Message = "parent comment => child comment 1",
                                Created = DateTime.Now.AddMinutes(5),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,
                            },
                            new Comment
                            {
                                Message = "parent comment => child comment 2",
                                Created = DateTime.Now.AddMinutes(10),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,

                                ChildComments = new List<Comment>
                                {
                                    new Comment
                                    {
                                        Message = "parent comment => child comment 2 => subchild comment 1",
                                        Created = DateTime.Now.AddMinutes(5),
                                        PostId = 1,
                                        UserId = superAdminId,
                                        //ParentCommentId = 1,
                                    },
                                    new Comment
                                    {
                                        Message = "parent comment => child comment 2 => subchild comment 2",
                                        Created = DateTime.Now.AddMinutes(10),
                                        PostId = 1,
                                        UserId = superAdminId,
                                        //ParentCommentId = 1,


                                    }
                                }
                            }
                        }
                    },
                    new Comment
                    {
                        Message = "This is a second parent comment",
                        Created = DateTime.Now.AddMinutes(1),
                        PostId = 1,
                        UserId = superAdminId,
                        ParentCommentId = null,

                        ChildComments = new List<Comment>
                        {
                            new Comment
                            {
                                Message = "parent comment => child comment 1",
                                Created = DateTime.Now.AddMinutes(5),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,
                            },
                            new Comment
                            {
                                Message = "parent comment => child comment 2",
                                Created = DateTime.Now.AddMinutes(10),
                                PostId = 1,
                                UserId = superAdminId,
                                //ParentCommentId = 1,
                            }
                        }
                    },
                    new Comment
                    {
                        Message = "This is a third parent comment still without child comments",
                        Created = DateTime.Now.AddMinutes(1),
                        PostId = 1,
                        UserId = superAdminId,
                        ParentCommentId = null,
                    }
                };
                // /////////////////////////////////////////////////////////////////


                IList<Post> posts = new List<Post>();
                string loremIpsum = LoremIpsum.loremIpsumString;

                Post fitstPost = new Post
                {
                    Title = "Example Title for Ukraine 1",
                    Description = "Example Description 1",
                    Body = loremIpsum,
                    Created = DateTime.Now,
                    MainPostImagePath = pictures[0], //ukrainePicture1,
                    CategoryId = 1,
                    UserId = superAdminId,
                    Comments = comments
                };

                posts.Add(fitstPost);

                for (int i = 2; i <= 10; i++)
                {
                    posts.Add(new Post
                    {
                        Title = $"Example Title for Ukraine {i}",
                        Description = (i % 3 == 0) ? null : $"Example Description {i}",
                        Body = loremIpsum,
                        Created = DateTime.Now,
                        MainPostImagePath = pictures[i - 1],
                        CategoryId = 1,
                        UserId = superAdminId,
                        // Comments = comments
                    });
                }

                for (int i = 11; i <= 20; i++)
                {
                    posts.Add(new Post
                    {
                        Title = $"Example Title for Music {i}",
                        Description = (i % 3 == 0) ? null : $"Example Description {i}",
                        Body = loremIpsum,
                        Created = DateTime.Now,
                        MainPostImagePath = pictures[i - 1],
                        CategoryId = 2,
                        UserId = superAdminId,
                        // Comments = comments
                    });
                }

                for (int i = 21; i <= 40; i++)
                {
                    posts.Add(new Post
                    {
                        Title = $"Example Title for Astronomy {i}",
                        Description = (i % 3 == 0) ? null : $"Example Description {i}",
                        Body = loremIpsum,
                        Created = DateTime.Now,
                        MainPostImagePath = pictures[i - 1],
                        CategoryId = 3,
                        UserId = superAdminId,
                        // Comments = comments
                    });
                }


                // await context.PostImages.AddRangeAsync(postEmptyImages);
                // await context.Comments.AddRangeAsync(comments);

                await context.Categories.AddRangeAsync(categories);
                await context.Posts.AddRangeAsync(posts);

                await context.SaveChangesAsync();
            }
        }
    }
}
