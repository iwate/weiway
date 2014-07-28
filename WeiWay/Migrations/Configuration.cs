namespace WeiWay.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WeiWay.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WeiWay.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WeiWay.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Messages.AddOrUpdate(x => x.Text,
                new Message { Text = "うぇーい↑↑" , Index = 0},
                new Message { Text = "うぇい?", Index = 1 },
                new Message { Text = "うぇいうぇい(｀・ω・´)", Index = 2 },
                new Message { Text = "うぇーい←", Index = 3 },
                new Message { Text = "(☝ ՞ਊ ՞)☝ｳｪｰｲwwwwwﾌｩ!", Index = 3 },
                new Message { Text = "(ｄ･ิω･ิｄ)ｵｩｲｪｰ♪", Index = 4},
                new Message { Text = "ｳｪｰｲｗｗｗｗﾜﾝﾁｬﾝｗｗｗｗｗ", Index = 5},
                new Message { Text = "ｯｼｬ‼︎ﾜﾝﾁｬﾝｱﾙﾃﾞ‼︎", Index = 6 },
                new Message{ Text = "ヽ(゜∀゜)ノうぇーい", Index = 7}
                );
            var users = context.Users.ToList();
            var messages = context.Messages.OrderBy(x => x.CreateTime).Take(8).ToList();
            messages.ForEach(delegate(Message message)
            {
                users.ForEach(delegate(ApplicationUser user)
                {
                    if (!user.Messages.Contains(message))
                    {
                        message.Users.Add(user);
                        user.Messages.Add(message);
                        context.Entry(message).State = EntityState.Modified;
                        context.Entry(user).State = EntityState.Modified;
                    }
                });
            });
           context.SaveChanges();

           if (!context.Roles.Any(r => r.Name == "Admin"))
           {
               var store = new RoleStore<IdentityRole>(context);
               var manager = new RoleManager<IdentityRole>(store);
               var role = new IdentityRole { Name = "Admin" };

               manager.Create(role);
           }
        }
    }

}
