using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using WeiWay.Models;

namespace WeiWay
{
    public partial class Startup
    {
        // 認証設定の詳細については、http://go.microsoft.com/fwlink/?LinkId=301864 を参照してください
        public void ConfigureAuth(IAppBuilder app)
        {
            // リクエストあたり 1 インスタンスのみを使用するように DB コンテキストとユーザー マネージャーを設定します
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // アプリケーションが Cookie を使用して、サインインしたユーザーの情報を格納できるようにします
            // また、サードパーティのログイン プロバイダーを使用してログインするユーザーに関する情報を、Cookie を使用して一時的に保存できるようにします
            // サインイン Cookie の設定
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // 次の行のコメントを解除して、サード パーティのログイン プロバイダーを使用したログインを有効にします
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");
            var twitterOptions = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions
            {
                ConsumerKey = "hBetfrIg5ZRDFzTJLLfh283bK",
                ConsumerSecret = "gvJuzBKdxIn1bqbyY8AkQqhmovLwFh6aRlYRoeqXrAJ2UA2zoB",
                Provider = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token", context.AccessToken, XmlSchemaString, "Twitter"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token_secret", context.AccessTokenSecret, XmlSchemaString, "Twitter"));
                        return System.Threading.Tasks.Task.FromResult(0);
                    }
                }
            };
            app.UseTwitterAuthentication(twitterOptions);

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }
        const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
    }
}