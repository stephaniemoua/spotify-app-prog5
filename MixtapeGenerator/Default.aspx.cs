
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Net.Http;
using SpotifyAPI.Web;


namespace Program4c
{
    public partial class _Default : Page
    {
        SpotifyClient spotify;

        protected void Page_Load(object sender, EventArgs e) { }

        // Function for 'search' button after click 
        // Uses Spotify Search API to find the root song 
        protected async void Button1_Submit_Click(object sender, EventArgs e)
        {
            // Get input from user via TextBox
            string song = Convert.ToString(TextBox1.Text);
            // Get input from user via TextBox
            string artist = Convert.ToString(TextBox2.Text);

            string searchSong = "";

            if (song.Length > 0 && artist.Length > 0)
            {
                searchSong = song + " " + artist;
            } // 

            else if (song.Length == 0) //no song input
            {
                // display an error message

            }
            else if (artist.Length > 0)
            {
                searchSong = song;
            }

            Label1.Text = "Searching for " + song;

            string CLIENTID = "replace-with-id";
            string CLIENTSECRET = "replace-with-secret";
            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(CLIENTID, CLIENTSECRET);
            var response = await new OAuthClient(config).RequestToken(request);
            spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            // [placeholder] catch Spotify connection errors

            //perform search. CAN REPLACE WITH USER INPUTTED REQUEST HERE
            var search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, searchSong));

            //Get tracks from search result
            var trackResults = spotify.PaginateAll(search.Tracks, (s) => s.Tracks);

            //add the first 5 results into a list. This shouldn't be needed with paginate all
           /* List<FullTrack> trackList = new List<FullTrack>();
            for (int i = 0; i <= 5; i++)
            {
                trackList.Add(enumerator.Current);
                await enumerator.MoveNextAsync();
            }*/

            string temp = "";
            //print list of first 5 items that appear in search result
            for (int i = 0; i < 5; i++)
            {
                if (trackResults.Result[i] != null)
                {
                    //at this point we want user to input a number
                    //Console.Write("Option " + i + ": \"" + trackList[i].Name + "\" by \"" + trackList[i].Artists[0].Name + "\"");
                    //Console.WriteLine(" From the album \"" + trackList[i].Album.Name + "\"");
                    temp = i + ": \"" + trackResults.Result[i].Name + "\" by \"" + trackResults.Result[i].Artists[0].Name 
                        + "\"" + " From the album \"" + trackResults.Result[i].Album.Name + "\"";
                }
            }

            // Each generated option is displayed as an option  
            // User must choose one option 
            Option1.Text = trackResults.Result[0].Name + "\" by \"" + trackResults.Result[0].Artists[0].Name + "\"" + " From the album \"" + trackResults.Result[0].Album.Name;
            Option2.Text = trackResults.Result[1].Name + "\" by \"" + trackResults.Result[1].Artists[0].Name + "\"" + " From the album \"" + trackResults.Result[1].Album.Name;
            Option3.Text = trackResults.Result[2].Name + "\" by \"" + trackResults.Result[2].Artists[0].Name + "\"" + " From the album \"" + trackResults.Result[2].Album.Name;
            Option4.Text = trackResults.Result[3].Name + "\" by \"" + trackResults.Result[3].Artists[0].Name + "\"" + " From the album \"" + trackResults.Result[3].Album.Name;
            Option5.Text = trackResults.Result[4].Name + "\" by \"" + trackResults.Result[4].Artists[0].Name + "\"" + " From the album \"" + trackResults.Result[4].Album.Name;

            // Matches the choice from the list 
            // choice = input from default.aspx
            int choice = 1;
            string trackID = trackResults.Result[choice].Id;
            string artistID = trackResults.Result[choice].Artists[choice].Id;

            //get the genres of the artist by searching for the exact artist name based on choice from user
            List<string> artistGenres = new List<string>();
            search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Artist, trackResults.Result[choice].Artists[0].Name));
            var artistResults = spotify.PaginateAll(search.Artists, (s) => s.Artists);

            //go through every artist until we find a matching artist ID.
            //This may be problematic if we run into a weird case where we get the ID but when searching by name the artist doesnt show up
            //I set i to 50 because I wasn't sure how to iterate through the whole ilist, 80% sure we will have a 99% chance we find the artist
            for (int i = 0; i < 50; i++)
            {
                if (artistResults.Result[i] == null)
                {
                    //if we ran out of results to look for?
                    break;
                }
                //to ensure we have the right artis
                if (artistResults.Result[i].Id == artistID)
                {
                    artistGenres = artistResults.Result[i].Genres;
                    break;
                }
            }

            // information for generating the reccomendations
            RecommendationsRequest recFinder = new RecommendationsRequest();
            recFinder.SeedTracks.Add(trackID);
            recFinder.SeedGenres.Add(artistGenres[0]);
            recFinder.SeedArtists.Add(artistID);

            //WE CAN CHANGE AMOUNT OF SONGS WE WANT TO GENERATE HERE
            recFinder.Limit = 20;

            //performt he recommendation search
            var recList = spotify.Browse.GetRecommendations(recFinder);

            Console.WriteLine("\nReccomendations found: ");

            string recommendations = "";
            for (int i = 0; i < recList.Result.Tracks.Count; i++)
            {
                string tmp = ("Song " + (i + 1) + ": \"" + recList.Result.Tracks[i].Name + "\" by " + recList.Result.Tracks[i].Artists[0].Name);
                recommendations.Concat(tmp);
                //maybe print the URL for a track here idk how to find it I'm happy with what is done so far.
            }

            RecLabel.Text = "Reccomendations found: " + recommendations;

        }


    }
}