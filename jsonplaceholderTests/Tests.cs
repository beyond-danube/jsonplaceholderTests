using NUnit.Framework;
using TestData;
using Helpers;
using RestSharp;
using System.IO;
using Newtonsoft.Json;

namespace Tests
{
    [TestFixture]
    public class AddRemoveUpdateTests
    {
        RequestHelper RequestHelper { get; set; }

        [SetUp]
        public void Init()
        {
            RequestHelper = new RequestHelper();
        }

        [TestCase(2, "New Post Title")]
        public void AddPostTest(int userId, string title)
        {
            Posts posts = RequestHelper.GetResponse<Posts>(TestDataUrls.Posts);

            Post post = new Post() { UserId = userId, Title = title };
            string json = JsonConvert.SerializeObject(post);

            RequestHelper.SetMethod(Method.POST);
            RequestHelper.SetStringBody(json);

            Post response = RequestHelper.GetResponse<Post>(TestDataUrls.Posts);

            Assert.AreEqual(title, response.Title);
            Assert.AreEqual(userId, response.UserId);
            Assert.AreEqual(posts.Count + 1, response.Id);
        }

        [TestCase(50, 2, "Updated Post Title")]
        public void UpdatePostTest(int postId, int userId, string title)
        {
            Post post = new Post() { Id = postId, UserId = userId, Title = title };
            string json = JsonConvert.SerializeObject(post);

            RequestHelper.SetMethod(Method.PUT);
            RequestHelper.SetStringBody(json);

            Post response = RequestHelper.GetResponse<Post>(Path.Combine(TestDataUrls.Posts, postId.ToString()));

            Assert.AreEqual(post.Title, response.Title);
            Assert.AreEqual(post.UserId, response.UserId);
        }

        [TestCase(50)]
        public void DeletePostTest(int postId)
        {
            RequestHelper.SetMethod(Method.DELETE);

            Post response = RequestHelper.GetResponse<Post>(Path.Combine(TestDataUrls.Posts, postId.ToString()));

            Assert.AreEqual(0, response.Id);
        }
    }


    [TestFixture]
    public class CommentsTests
    {
        RequestHelper RequestHelper { get; set; }
        Comments Comments { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            RequestHelper = new RequestHelper();
            Comments = RequestHelper.GetResponse<Comments>(TestDataUrls.Comments);
        }

        [TestCase("ipsum dolorem", "Marcia@name.biz")]
        [TestCase("ipsum dolorem", "Jackeline@eva.tv")]
        public void EmailCheckByCommentContent(string bodyContent, string expectedEmail)
        {
            string actualEmail = Comments.Find(x => x.Body.Contains(bodyContent)).Email;

            Assert.AreEqual(expectedEmail, actualEmail);
        }
    }

    [TestFixture]
    public class PostsTests
    {
        RequestHelper RequestHelper { get; set; }
        Posts Posts { get; set; }
        Users Users { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            RequestHelper = new RequestHelper();
            Posts = RequestHelper.GetResponse<Posts>(TestDataUrls.Posts);
            Users = RequestHelper.GetResponse<Users>(TestDataUrls.Users);
        }

        [TestCase("eos dolorem iste accusantium est eaque quam", "Patricia Lebsack")]
        [TestCase("eos dolorem iste accusantium est eaque quam", "Someone Else")]
        public void CheckUserByPostTitle(string titleContent, string expectedUser)
        {
            int actualUserId = Posts.Find(x => x.Title.Contains(titleContent)).UserId;
            string actualUser = Users.Find(x => x.Id == actualUserId).Name;

            Assert.AreEqual(expectedUser, actualUser);
        }
    }

    [TestFixture]
    public class PhotosTests
    {
        RequestHelper RequestHelper { get; set; }
        Photos Photos { get; set; }
        Users Users { get; set; }
        Albums Albums { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            RequestHelper = new RequestHelper();
            Photos = RequestHelper.GetResponse<Photos>(TestDataUrls.Photos);
            Users = RequestHelper.GetResponse<Users>(TestDataUrls.Users);
            Albums = RequestHelper.GetResponse<Albums>(TestDataUrls.Albums);
        }

        [TestCase("ad et natus qui", "Sincere@april.biz")]
        [TestCase("ad et natus qui", "Anotherone@april.biz")]
        public void CheckUserByPhotoTitle(string photoTitle, string expectedEmail)
        {
            int albumId = Photos.Find(x => x.Title.Contains(photoTitle)).AlbumId;
            int userId = Albums.Find(x => x.Id == albumId).UserId;

            string actualEmail = Users.Find(x => x.Id == userId).Email;

            Assert.AreEqual(expectedEmail, actualEmail);
        }

        [TestCase(4, "BinaryTestReferences", "magenta600x600.png")]
        [TestCase(4, "BinaryTestReferences", "magenta600x600_corrupted.png")]
        public void CheckPhotoUsingRefImage(int imageId, string refImageFolder, string refImageFile)
        {

            string expectedImage = Path.Combine(Directory.GetCurrentDirectory(), refImageFolder, refImageFile);
            string actualImage = Path.Combine(Directory.GetCurrentDirectory(), refImageFolder, imageId.ToString() + Path.GetExtension(expectedImage));

            Photo response = RequestHelper.GetResponse<Photo>(Path.Combine(TestDataUrls.Photos, imageId.ToString()));

            RequestHelper.SaveImage(response.Url, actualImage);

            Assert.IsTrue(BinaryFileComparer.FilesAreEqual(expectedImage, actualImage));
        }
    }

    [TestFixture]
    public class TodosTests
    {
        RequestHelper RequestHelper { get; set; }
        Todos Todos { get; set; }
        Users Users { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            RequestHelper = new RequestHelper();
            Todos = RequestHelper.GetResponse<Todos>(TestDataUrls.Todos);
            Users = RequestHelper.GetResponse<Users>(TestDataUrls.Users);
        }

        [TestCase("Leanne Graham", "Ervin Howell", 3, true)]
        [TestCase("Leanne Graham", "Ervin Howell", 2, true)]
        public void CompareTodosBetweenUsers(string user1, string user2, int expectedDiff, bool completed)
        {
            int user1Id = Users.Find(x => x.Name == user1).Id;
            int user2Id = Users.Find(x => x.Name == user2).Id;

            int user1CompletedCount = Todos.FindAll(x => x.UserId == user1Id && x.Completed == completed).Count;
            int user2CompletedCount = Todos.FindAll(x => x.UserId == user2Id && x.Completed == completed).Count;

            int actualDiff = user1CompletedCount - user2CompletedCount;

            Assert.Greater(actualDiff, expectedDiff);
        }
    }

}