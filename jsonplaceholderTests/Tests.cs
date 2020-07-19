using TestData;
using Helpers;
using RestSharp;
using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Tests
{
    public class AddRemoveUpdateTests
    {
        RequestHelper RequestHelper { get; set; }

        public AddRemoveUpdateTests()
        {
            RequestHelper = new RequestHelper();
        }

        [Theory]
        [InlineData(2, "New Post Title")]
        public void AddPostTest(int userId, string title)
        {
            Posts posts = RequestHelper.GetResponse<Posts>(TestDataUrls.Posts);

            Post post = new Post() { UserId = userId, Title = title };
            string json = JsonConvert.SerializeObject(post);

            RequestHelper.SetMethod(Method.POST);
            RequestHelper.SetStringBody(json);

            Post response = RequestHelper.GetResponse<Post>(TestDataUrls.Posts);

            Assert.Equal(title, response.Title);
            Assert.Equal(userId, response.UserId);
            Assert.Equal(posts.Count + 1, response.Id);
        }

        [Theory]
        [InlineData(50, 2, "Updated Post Title")]
        public void UpdatePostTest(int postId, int userId, string title)
        {
            Post post = new Post() { Id = postId, UserId = userId, Title = title };
            string json = JsonConvert.SerializeObject(post);

            RequestHelper.SetMethod(Method.PUT);
            RequestHelper.SetStringBody(json);

            Post response = RequestHelper.GetResponse<Post>(Path.Combine(TestDataUrls.Posts, postId.ToString()));

            Assert.Equal(post.Title, response.Title);
            Assert.Equal(post.UserId, response.UserId);
        }

        [Theory]
        [InlineData(50)]
        public void DeletePostTest(int postId)
        {
            RequestHelper.SetMethod(Method.DELETE);

            Post response = RequestHelper.GetResponse<Post>(Path.Combine(TestDataUrls.Posts, postId.ToString()));

            Assert.Equal(0, response.Id);
        }
    }

    public class CommentsTests
    {
        RequestHelper RequestHelper { get; set; }
        Comments Comments { get; set; }

        public CommentsTests()
        {
            RequestHelper = new RequestHelper();
            Comments = RequestHelper.GetResponse<Comments>(TestDataUrls.Comments);
        }

        [Theory]
        [InlineData("ipsum dolorem", "Marcia@name.biz")]
        [InlineData("ipsum dolorem", "Jackeline@eva.tv")]
        public void EmailCheckByCommentContent(string bodyContent, string expectedEmail)
        {
            string actualEmail = Comments.Find(x => x.Body.Contains(bodyContent)).Email;

            Assert.Equal(expectedEmail, actualEmail);
        }
    }

    public class PostsTests
    {
        RequestHelper RequestHelper { get; set; }
        Posts Posts { get; set; }
        Users Users { get; set; }

        public PostsTests()
        {
            RequestHelper = new RequestHelper();
            Posts = RequestHelper.GetResponse<Posts>(TestDataUrls.Posts);
            Users = RequestHelper.GetResponse<Users>(TestDataUrls.Users);
        }

        [Theory]
        [InlineData("eos dolorem iste accusantium est eaque quam", "Patricia Lebsack")]
        [InlineData("eos dolorem iste accusantium est eaque quam", "Someone Else")]
        public void CheckUserByPostTitle(string titleContent, string expectedUser)
        {
            int actualUserId = Posts.Find(x => x.Title.Contains(titleContent)).UserId;
            string actualUser = Users.Find(x => x.Id == actualUserId).Name;

            Assert.Equal(expectedUser, actualUser);
        }
    }

    public class PhotosTests
    {
        RequestHelper RequestHelper { get; set; }
        Photos Photos { get; set; }
        Users Users { get; set; }
        Albums Albums { get; set; }

        public PhotosTests()
        {
            RequestHelper = new RequestHelper();
            Photos = RequestHelper.GetResponse<Photos>(TestDataUrls.Photos);
            Users = RequestHelper.GetResponse<Users>(TestDataUrls.Users);
            Albums = RequestHelper.GetResponse<Albums>(TestDataUrls.Albums);
        }

        [Theory]
        [InlineData("ad et natus qui", "Sincere@april.biz")]
        [InlineData("ad et natus qui", "Anotherone@april.biz")]
        public void CheckUserByPhotoTitle(string photoTitle, string expectedEmail)
        {
            int albumId = Photos.Find(x => x.Title.Contains(photoTitle)).AlbumId;
            int userId = Albums.Find(x => x.Id == albumId).UserId;

            string actualEmail = Users.Find(x => x.Id == userId).Email;

            Assert.Equal(expectedEmail, actualEmail);
        }

        [Theory]
        [InlineData(4, "BinaryTestReferences", "magenta600x600.png")]
        [InlineData(4, "BinaryTestReferences", "magenta600x600_corrupted.png")]
        public void CheckPhotoUsingRefImage(int imageId, string refImageFolder, string refImageFile)
        {

            string expectedImage = Path.Combine(Directory.GetCurrentDirectory(), refImageFolder, refImageFile);
            string actualImage = Path.Combine(Directory.GetCurrentDirectory(), refImageFolder, imageId.ToString() + Path.GetExtension(expectedImage));

            Photo response = RequestHelper.GetResponse<Photo>(Path.Combine(TestDataUrls.Photos, imageId.ToString()));

            RequestHelper.SaveImage(response.Url, actualImage);

            var comparer = new BinaryFileComparer();

            Assert.Equal(expectedImage, actualImage, comparer);
        }
    }

    public class TodosTests
    {
        RequestHelper RequestHelper { get; set; }
        Todos Todos { get; set; }
        Users Users { get; set; }

        public TodosTests()
        {
            RequestHelper = new RequestHelper();
            Todos = RequestHelper.GetResponse<Todos>(TestDataUrls.Todos);
            Users = RequestHelper.GetResponse<Users>(TestDataUrls.Users);
        }

        [Theory]
        [InlineData("Leanne Graham", "Ervin Howell", 3, true)]
        [InlineData("Leanne Graham", "Ervin Howell", 2, true)]
        public void CompareTodosBetweenUsers(string user1, string user2, int expectedDiff, bool completed)
        {
            int user1Id = Users.Find(x => x.Name == user1).Id;
            int user2Id = Users.Find(x => x.Name == user2).Id;

            int user1CompletedCount = Todos.FindAll(x => x.UserId == user1Id && x.Completed == completed).Count;
            int user2CompletedCount = Todos.FindAll(x => x.UserId == user2Id && x.Completed == completed).Count;

            int actualDiff = user1CompletedCount - user2CompletedCount;

            Assert.True(actualDiff > expectedDiff);
        }
    }

}