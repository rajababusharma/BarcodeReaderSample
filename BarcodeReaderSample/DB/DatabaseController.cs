using SQLite;
using BarcodeReaderSample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using MigraDoc.DocumentObjectModel;

namespace BarcodeReaderSample.DB
{
    public class DatabaseController
    {
        SQLiteAsyncConnection Database;
       // private SQLiteConnection database;

        public DatabaseController()
        {
            
        }
        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(DBConstant.DatabasePath, DBConstant.Flags);
            var users = await Database.CreateTableAsync<Users>();
            var articles = await Database.CreateTableAsync<Articles>();
        }
       
        public async Task<int> Insert_IntoUsers(Users user)
        {  
                await Init();
                return await Database.InsertOrReplaceAsync(user);
        }
        public async Task<int> Insert_IntoARTICLES(Articles Mylist)
        {
            await Init();
            return await Database.InsertOrReplaceAsync(Mylist);
            
        }

        public async Task<int> Update_Articles(string article,string dt,int count,string user)
        {
            Articles articles = new Articles();
            articles.ARTICLES = article;
            articles.USER = user;
            articles.DATETIME = dt;
            await Init();
                return await Database.InsertOrReplaceAsync(articles);
        }

        public async Task<List<Articles>> GetAll_Articles()
        {
            List<Articles> articles = new List<Articles>();
             Init();
            return await  Database.Table<Articles>().OrderByDescending(x=>x.DATETIME).ToListAsync();
           // return articles.OrderByDescending(x => x.DATETIME).ToList();
        }
        public async void Delete_AllArticles()
        {
            await Init();

            await Database.DeleteAllAsync<Articles>();
            //var articles = await Database.CreateTableAsync<Articles>();

        }
        public async Task<int> CheckArticleDuplicate(string article)
        {
            await Init();
            int flag = 0;
            List<Articles> Mylist = null;

            // Mylist = new List<AUSAddShortAccessEnitity>();
            // Mylist = database.Query<DocketDetList>("Select Docket_No,Articles,Scanned_status From DocketDetList");
            //var existingItem = dbConn.Get<DocketDetList>(dock);
            // Mylist = dbConn.Query<ArticlesLoading>("Select * From ArticlesLoading where ARTICLE_ID=" + "'" + article + "'");
            Mylist = await Database.QueryAsync<Articles>("Select * From Articles where ARTICLES=" + "'" + article + "'");
           // Mylist = await Database.QueryAsync<Articles>("Select * From Articles where ARTICLES=" + "'" + article + "'");
                        if (Mylist.Count>0)
                        {
                            flag = 1;
                        }
                      
                return flag;  
        }
        public async Task<int> Authenticate(Users user)
        {
            await Init();
            int flag = 0;
            List<Users> Mylist = null;
           
                        // Mylist = new List<AUSAddShortAccessEnitity>();
                        // Mylist = database.Query<DocketDetList>("Select Docket_No,Articles,Scanned_status From DocketDetList");
                        //var existingItem = dbConn.Get<DocketDetList>(dock);
                        // Mylist = dbConn.Query<ArticlesLoading>("Select * From ArticlesLoading where ARTICLE_ID=" + "'" + article + "'");
                        Mylist = await Database.QueryAsync<Users>("Select * From Users where username=" + "'" + user.username + "' and password=" + "'" + user.password + "'");
                        if (Mylist.Count > 0)
                        {
                           
                                flag = 1;
                        }
   
                   
                return flag;
           

        }
        public async Task<int> GetTotalArticleCount()
        {
            await Init();
          
            int flag = 0;
            List<Articles> Mylist = null;
           
                       
                        Mylist = await Database.QueryAsync<Articles>("Select * From Articles");
                        if (Mylist.Count > 0)
                        {
                            flag = Mylist.Count;
                        }
                       
                return flag;
            

        }
        public async Task<List<Users>> GetAll_Users()
        {
            await Init();
            return await Database.Table<Users>().ToListAsync();            
        }
    }
}
