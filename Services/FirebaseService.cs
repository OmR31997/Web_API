using EntertaimentLib_API.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace EntertaimentLib_API.Services
{
    public interface IFeedBackService
    {
        Task<List<FeedBack>> FeedBacksGetAsy(string path, string EntId);
        Task<string> FeedBackPostAsy(string path, FeedBack newFeedBack);
        Task<string> FeedBackLikePatchAsy(string path, bool newLikeValue);
    }

    public class FirebaseService: IFeedBackService
    {
        private readonly FirebaseClient _FClient;
        public FirebaseService() => _FClient = new FirebaseClient("https://entertaimentlib-ad956-default-rtdb.firebaseio.com/");


        #region Get All Movies From Firebase
        public async Task<List<Movie?>> GetAllAsy<Movie>(string path) where Movie : class
        {
            var movies = await _FClient.Child(path).OnceAsync<Movie>();

            if (movies == null || !movies.Any())
            {
                throw new Exception("No data available.");
            }

            try
            {
                // Assign the Firebase key to each movie's Id property
                var movieList = movies.Select(m =>
                {
                    var movie = m.Object as dynamic; // Cast to dynamic to access properties
                    movie.Id = m.Key; // Assign the Firebase key to the Id property
                    return movie as Movie; // Return the movie object
                }).ToList();

                return movieList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Get Movie From Firebase Through Id
        public async Task<Movie?> GetMovieByIdAsync(string? path, string? Id)
        {
            // the movie's Details Copy  from the 'Movies' node
            var movie = await _FClient.Child(path).Child(Id).OnceSingleAsync<Movie>();

            if (movie == null)
            {
                return null;
            }

            try
            {
                movie.Id = Id;
                return movie;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching movie with ID {Id}: {ex.Message}");
            }
        }

        #endregion

        #region Movie Posting With Custom Id
        public async Task<string> PostMovieWithCustomIdAsync(Movie newMovie)
        {
            // Generate a custom ID
            string customId = await GenerateCustomIdAsync(newMovie.Location);
            try
            {
                // Use PUT to insert the movie with the custom ID

                await _FClient.Child("movies").Child(customId).PutAsync(newMovie);

                // Return the custom ID used
                return customId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting movie with custom ID {customId}: {ex.Message}");
            }
        }


        private async Task<string> GenerateCustomIdAsync(string? location)
        {
            string loc = location ?? string.Empty;

            // Get the first three letters of the location
            var locAbbreviation = loc.Length < 2 ? loc.ToUpper() : loc.Substring(0, 3).ToUpper();

            // Get all movies for the current location
            var movies = await _FClient.Child("movies")
                                        .OrderByKey()
                                        .StartAt(locAbbreviation + "CNM") // Ensures we start looking from the right prefix
                                        .OnceAsync<Movie>();

            int nextIdNumber = 1;

            if (movies.Any())
            {
                if (movies != null)
                {
                    // Extract the keys that match the prefix for the location
                    var matchingMovies = movies.Where(m => m.Key.StartsWith(locAbbreviation + "CNM"));

                    var lastMovieKey = (matchingMovies.Any() ? matchingMovies.OrderByDescending(m => m.Key).First().Key : loc);

                    // Get the last movie key

                    // Extract the numeric part of the last custom ID
                    string lastIdNumber = lastMovieKey.Replace(locAbbreviation + "CNM", "");

                    // Try to parse the numeric part
                    if (int.TryParse(lastIdNumber, out int idNum))
                    {
                        nextIdNumber = idNum + 1; // Increment the number for the next ID
                    }
                }
            }

            // Return the new custom ID in the format 'LOC0001'
            return $"{locAbbreviation}CNM{nextIdNumber:D3}"; // Zero-padded to 3 digits
        }

        #endregion

        #region Movie Posting With Default Firebase Id
        public async Task<string> PostAsy<Movie>(Movie newMovie)
        {
            try
            {
                var movie = await _FClient.Child("movies").PostAsync<Movie?>(newMovie);
                return movie.Key;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }
        #endregion

        #region Update Movie Details Into Firebase 
        public async Task<bool> PutAsy<Movie>(string path, Movie movie)
        {
            try
            {
                if (await _FClient.Child(path).OnceSingleAsync<Movie>() == null)
                    return false;
                else
                {
                    await _FClient.Child(path).PutAsync(movie);
                    return true; // Operation succeeded
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating data in Firebase: {ex.Message}");
            }
        }
        #endregion

        #region Delete Movie Movie From Firebase Through Id
        public async Task<bool> DeleteAsy<Movie>(string path)
        {
            try
            {
                if (await _FClient.Child(path).OnceSingleAsync<Movie>() != null)
                {
                    await _FClient.Child(path).DeleteAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting data in Firebase: {ex.Message}");
            }
        }
        #endregion



        #region Get All Show From Firebase
        public async Task<List<Show?>> GetAllShowsAsy<Show>(string path) where Show : class
        {
            var shows = await _FClient.Child(path).OnceAsync<Show>();

            if (shows == null || !shows.Any())
            {
                throw new Exception("No data available.");
            }

            try
            {
                // Assign the Firebase key to each show's Id property
                var showList = shows.Select(m =>
                {
                    var show = m.Object as dynamic; // Cast to dynamic to access properties
                    show.Id = m.Key; // Assign the Firebase key to the Id property
                    return show as Show; // Return the show object
                }).ToList();

                return showList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Get Show From Firebase Through Id
        public async Task<Show?> GetShowByIdAsync(string? path, string? Id)
        {
            var show = await _FClient.Child(path).Child(Id).OnceSingleAsync<Show>();
            if (show == null)
            {
                return null;
            }

            try
            {
                show.Id = Id;
                return show;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching show with ID {Id}: {ex.Message}");
            }
        }

        #endregion

        #region Show Posting With Custom Id
        public async Task<string> PostShowWithCustomIdAsync(Show newShow)
        {
            // Generate a custom ID
            string customId = await GenerateShowCustomIdAsync(newShow.Location, newShow.ShowType);
            try
            {
                // Use PUT to insert the movie with the custom ID
                await _FClient.Child("shows").Child(customId).PutAsync(newShow);
                // Return the custom ID used
                return customId;
            }
            catch (Exception ex)
            {
                // Consider logging the exception and throwing a custom exception
                throw new Exception($"Error posting show with custom ID {customId}: {ex.Message}", ex);
            }
        }

        private async Task<string> GenerateShowCustomIdAsync(string? location, string? showType)
        {
            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(showType))
            {
                throw new ArgumentException("Location and ShowType cannot be null or empty.");
            }

            string loc = location ?? string.Empty;

            // Get the first three letters of the location
            loc = loc.Length < 2 ? loc.ToUpper() : loc.Substring(0, 3).ToUpper();

            var showTypeParts = showType.Split(' '); // Ensure valid separator
            if (showTypeParts.Length < 2)
            {
                throw new ArgumentException("ShowType must contain at least two parts.");
            }

            var sShowType = $"{showTypeParts[0][0]}{showTypeParts[1][0]}";

            var shows = await _FClient.Child("shows")
                                        .OrderByKey()
                                        .StartAt(loc + sShowType)
                                        .OnceAsync<Show>();

            int nextIdNumber = 1;

            if (shows.Any())
            {
                var matchingShows = shows.Where(m => m.Key.StartsWith(loc + sShowType)).ToList();

                if (matchingShows.Any())
                {
                    var lastShowKey = matchingShows.OrderByDescending(m => m.Key).First().Key;
                    string lastIdNumber = lastShowKey.Replace(loc + sShowType, "");

                    if (int.TryParse(lastIdNumber, out int idNum))
                    {
                        nextIdNumber = idNum + 1; // Increment the number for the next ID
                    }
                }
            }

            return $"{loc + sShowType}{nextIdNumber:D3}"; // Zero-padded to 3 digits
        }

        #endregion

        #region Show Posting With Default Firebase Id
        public async Task<string> PostShowAsy<Show>(Show newShow)
        {
            try
            {
                var show = await _FClient.Child("shows").PostAsync<Show>(newShow);
                return show.Key;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }
        #endregion

        #region Update Show Details Into Firebase 
        public async Task<bool> PutShowAsy<Show>(string path, Show show)
        {
            try
            {
                if (await _FClient.Child(path).OnceSingleAsync<Show>() == null)
                    return false;
                else
                {
                    await _FClient.Child(path).PutAsync(show);
                    return true; // Operation succeeded
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating data in Firebase: {ex.Message}");
            }
        }
        #endregion

        #region Delete Show Details From Firebase Through Id
        public async Task<bool> DeleteShowAsy<Movie>(string path)
        {
            try
            {
                if (await _FClient.Child(path).OnceSingleAsync<Show>() != null)
                {
                    await _FClient.Child(path).DeleteAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting data in Firebase: {ex.Message}");
            }
        }
        #endregion



        #region FeedBack For Movies/Show

        public async Task<List<FeedBack>> Feeds(string path)
        {
            var feeds = await _FClient.Child(path).OnceAsync<FeedBack>();
            List<FeedBack> list = new List<FeedBack>();
            if (feeds == null || !feeds.Any())
            {
                return list;
            }

            try
            {
                foreach (var feed in feeds)
                {
                    list.Add(
                        new FeedBack
                        {
                            Fid = feed.Key,
                            Email = feed.Object.Email,
                            EntId = feed.Object.EntId,
                            Like = feed.Object.Like,
                            Comment = await this.Comments(path, feed.Key),
                            Rating = feed.Object.Rating
                        });
                }
                return list;
            }
            catch (Exception ex)
            {
                // Log detailed error information
                Console.WriteLine($"Error fetching feedbacks: {ex.Message}");
                throw new Exception("An error occurred while fetching feedbacks.", ex);
            }
        }

        public async Task<List<Comment>> Comments(string path, string fid)
        {
            try
            {
                // Use OnceSingleAsync<List<Comment>>() to handle array response
                var comments = await _FClient.Child(path).Child(fid).Child("Comment").OnceSingleAsync<List<Comment>>();

                if (comments == null || !comments.Any())
                {
                    Console.WriteLine("No comments found for the specified feedback ID.");
                    return new List<Comment>(); // Return an empty list if there are no comments
                }

                // Firebase should populate the CmtId for each comment automatically
                for (int i = 0; i < comments.Count; i++)
                {
                    comments[i].CmtId = i.ToString(); // Assign a sequential CmtId to each comment, if needed
                }

                return comments;
            }
            catch (Exception ex)
            {
                // Log detailed error information
                Console.WriteLine($"Error fetching comments: {ex.Message}");
                throw new Exception("An error occurred while fetching comments.", ex);
            }
        }

        public async Task<List<FeedBack>> FeedBacksGetAsy(string path, string EntId)
        {
            try
            {
                var feeds = this.Feeds(path);

                List<FeedBack> EntIdBasedFeeds = new List<FeedBack>();

                foreach (var feed in await feeds)
                {
                    if (feed.Fid != null)
                    {
                        // Retrieve comments asynchronously and wait for completion
                        var comments = await this.Comments(path, feed.Fid);


                        if (feed.EntId == EntId)
                        {
                            EntIdBasedFeeds.Add(new FeedBack
                            {
                                Fid = feed.Fid,
                                Email = feed.Email,
                                EntId = feed.EntId,
                                Like = feed.Like,
                                Comment = await this.Comments(path, feed.Fid),
                                Rating = feed.Rating
                            });
                        }
                    }
                }

                return EntIdBasedFeeds;
            }
            catch (Exception ex)
            {
                // Log detailed error information
                Console.WriteLine($"Error fetching feedbacks: {ex.Message}");
                throw new Exception("An error occurred while fetching feedbacks.", ex);
            }
        }


        public async Task<string> FeedBackPostAsy(string path, FeedBack newFeedBack)
        {
            try
            {
                var feedback = await _FClient.Child(path).PostAsync<FeedBack>(newFeedBack);
                return feedback.Key;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }

        public async Task<string> FeedBackLikePatchAsy(string path, bool newLikeValue)
        {
            try
            {
                var patchData = new Dictionary<string, bool> { { "Like", newLikeValue } };
                await _FClient.Child(path).PatchAsync(patchData);
                return "The Like status has been updated";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }

        public async Task<string> FeedBackCmtPatchAsy(string path, Comment newCmtValue)
        {
            try
            {
                //var patchData = new Dictionary<string, Object> { { "Comment", newCmtValue } };
                await _FClient.Child(path).PatchAsync(newCmtValue);
                return "The Comment status has been updated";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }

        public async Task<string> FeedBackRatingPatchAsy(string path, short newRatingValue)
        {
            try
            {
                var patchData = new Dictionary<string, short> { { "Rating", newRatingValue } };
                await _FClient.Child(path).PatchAsync(patchData);
                return "The Rating status has been updated";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }

        public async Task<string> CmtEdit(string path, Comment updCmt)
        {
            try
            {
                await _FClient.Child(path).PatchAsync(updCmt);
                return "The Comment status has been updated";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }

        public async Task<string> DelCmt(string path, int CmtId)
        {
            try
            {
                //await _FClient.Child(path).Child(CmtId).DeleteAsync();

                // Step 1: Fetch all comments as a list
                var comments = await _FClient.Child(path).OnceSingleAsync<List<Comment>>();

                // Check if the delete index is valid
                if (comments == null || CmtId < 0 || CmtId >= comments.Count)
                {
                    throw new Exception("Invalid index or no comments found.");
                }

                // Step 2: Remove the comment at the specified index
                comments.RemoveAt(CmtId);

                // Step 3: Reindex comments list and re-upload to Firebase
                await _FClient.Child(path).PutAsync(comments);

                return "Comment has been deleted"; // Return the reindexed list
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }



        #endregion


        #region For User Records
        #region Get User
        public async Task<string> GetPassword(string path)
        {
            try
            {
                UserRecord user = await _FClient.Child(path).OnceSingleAsync<UserRecord>();
                if (user.Password != null)
                {
                    return user.Password;
                }
                return BCrypt.Net.BCrypt.HashPassword("nothing");
            }
            catch
            {
                return BCrypt.Net.BCrypt.HashPassword("nothing");
            }
        }
        #endregion

        #region Get Users
        public async Task<List<UserRecord>> GetUsersAsy(string path)
        {
            var users = await _FClient.Child(path).OnceAsync<UserRecord>();
            List<UserRecord> list = new List<UserRecord>();
            if (users == null || !users.Any())
            {
                return list;
            }

            try
            {
                foreach (var user in users)
                {
                    list.Add(
                        new UserRecord
                        {
                            UserId = user.Key,
                            UserName = user.Object.UserName,
                            MNo = user.Object.MNo,
                            Email = user.Object.Email,
                            Password = (user.Object.Password != null) ? "HasPassword" : "Pending",
                            WatchList = await this.GetWatchList($"users", user.Key),
                            Created = user.Object.Created
                        });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting from Firebase: {ex.Message}");
            }
        }
        #endregion

        public async Task<string> UserPasswordAsy(string path, string password)
        {
            try
            {
                password= BCrypt.Net.BCrypt.HashPassword(password);
                var patchData = new Dictionary<string, string> { { "Password", password } };
                await _FClient.Child(path).PatchAsync(patchData);
                return password;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }
        public async Task<List<UserRecord>> GetSecured(string path)
        {
            var users = await _FClient.Child(path).OnceAsync<UserRecord>();

            List<UserRecord> list = new List<UserRecord>();
            if (users == null || !users.Any())
            {
                return list;
            }

            try
            {
                foreach (var user in users)
                {
                    list.Add(
                        new UserRecord
                        {
                            UserId = user.Key,
                            UserName = user.Object.UserName,
                            MNo = user.Object.MNo,
                            Email = user.Object.Email,
                            Password = user.Object.Password,
                            WatchList = await this.GetWatchList($"users", user.Key),
                            Created = user.Object.Created
                        });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting from Firebase: {ex.Message}");
            }
        }

        #region Get Last User
        public async Task<string> GetLastUser(string path)
        {
            var users = await _FClient.Child(path).OrderByKey().OnceAsync<Models.UserRecord>();
            var lastUserKey = "UENT";
            try
            {
                if (users == null || !users.Any())
                {
                    // Handle the case where no records were found
                    Console.WriteLine("No user records found.");
                    return lastUserKey; // Return an empty list or handle as needed
                }
                else
                {
                    lastUserKey = users.OrderByDescending(user => user.Key).First().Key;
                }
                return lastUserKey;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error to get last user from Firebase: {ex.Message}");
            }
        }
        #endregion

        #region User Post
        public async Task<UserRecord> UserPostAsy(string path, UserRecord newUser, string customId)
        {
            try
            {
                if (newUser.UserId == null)
                {
                    // Hash the password only if it’s defined
                    if (!string.IsNullOrEmpty(newUser.Password))
                    {
                        newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                    }

                    await _FClient.Child(path).Child(customId).PutAsync(newUser);
                    UserRecord user = new UserRecord();
                    var result = await _FClient.Child(path).Child(customId).OnceSingleAsync<UserRecord>();
                    user.UserId = customId;
                    user.UserName = result.UserName;
                    user.MNo = result.MNo;
                    user.Email = result.Email;
                    user.Password = result.Password;
                    user.WatchList = result.WatchList;
                    user.Created = result.Created;

                    return user;
                }
                else
                    return new UserRecord();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }
        #endregion

        #region User Update
        public async Task<bool> UserPutAsy(string path, UserRecord updUser)
        {
            try
            {
                var user = await _FClient.Child(path).OnceSingleAsync<UserRecord>();
                if (user.Password != null)
                {
                    updUser.Password = user.Password;
                }

                await _FClient.Child(path).PutAsync<UserRecord>(updUser);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }
        #endregion

        #region WatchList
        public async Task<List<WatchList>> GetWatchList(string path, string UserId)
        {
            try
            {
                // Use OnceSingleAsync<List<Comment>>() to handle array response
                var watchList = await _FClient.Child(path).Child(UserId).Child("WatchList").OnceSingleAsync<List<WatchList>>();

                if (watchList == null || !watchList.Any())
                {
                    Console.WriteLine("WatchListed by zero User");
                    return new List<WatchList>(); // Return an empty list if there are no comments
                }

                // Firebase should populate the CmtId for each comment automatically
                for (int i = 0; i < watchList.Count; i++)
                {
                    watchList[i].WatchId = i.ToString(); // Assign a sequential CmtId to each comment, if needed
                }

                return watchList;
            }
            catch (Exception ex)
            {
                // Log detailed error information
                Console.WriteLine($"Error fetching comments: {ex.Message}");
                throw new Exception("An error occurred while fetching comments.", ex);
            }
        }

        public async Task<string> WatchListPatchAsy(string path, WatchList fav)
        {
            try
            {
                //var patchData = new Dictionary<string, Object> { { "WatchList", fav } };
                await _FClient.Child(path).PatchAsync(fav);
                return "This item has been taken as WatchList";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }
        }

        #endregion

        #region Delete User
        public async Task<bool> UserDeleteAsy(string path, string UserId)
        {
            try
            {
                await _FClient.Child(path).Child(UserId).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to Firebase: {ex.Message}");
            }

        }
        #endregion

        #endregion
    }
}
