﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChessClient
{
    public class Client
    {
        public enum Result
        {
            BadRequest = HttpStatusCode.BadRequest,
            NotFound = HttpStatusCode.NotFound,
            Ok = HttpStatusCode.OK,
            Created = HttpStatusCode.Created
        }

        string host;

        public Client(string host)
        {
            this.host = host;
        }

        async Task RequestPost<T1, T2>(string entity, string method, T1 entry, Action<Result, T2> callback)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = host + entity + method;

                string entryJSON = JsonConvert.SerializeObject(entry);
                StringContent entryContent = new StringContent(entryJSON);
                entryContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (HttpResponseMessage response = await client.PostAsync(requestUrl, entryContent))
                {
                    Result r = (Result)response.StatusCode;
                    
                    if (r == Result.Created || r == Result.Ok)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string resultJSON = await content.ReadAsStringAsync();
                            T2 result = JsonConvert.DeserializeObject<T2>(resultJSON);
                            callback?.Invoke(r, result);
                        }
                    }
                    else
                    {
                        callback?.Invoke(r, default(T2));
                    }
                }
            }
        }

        async Task RequestGet<T>(string entity, string method, Action<Result, T> callback)
        {
            string requestUrl = host + entity + method;

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(requestUrl))
                {
                    Result r = (Result)response.StatusCode;

                    if (r == Result.Created || r == Result.Ok)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string resultJSON = await content.ReadAsStringAsync();
                            T result = JsonConvert.DeserializeObject<T>(resultJSON);
                            callback?.Invoke(r, result);
                        }
                    }
                    else
                    {
                        callback?.Invoke(r, default(T));
                    }
                }
            }
        }


        // returns the player with player.GUID or creates a new one
        public async Task GetPlayer(Player player, Action<Result, PlayerInfo> callback)
        {
            await RequestPost("players", "/", player, callback);
        }

        // returns the player with player.GUID or creates a new one
        public async Task GetPlayer(int gameID, string color, Action<Result, PlayerInfo> callback)
        {
            await RequestGet("players", "/" + gameID.ToString() + "/" + color + "/", callback);
        }

        // returns a new game state after applying the 'move'
        public async Task SendMove(MoveInfo move, Action<Result, GameState> callback)
        {
            await RequestPost("moves", "/", move, callback);
        }

        // returns a game with 'wait' status; we can create a new game or join an existing game
        public async Task FindGame(RequestedGame r, Action<Result, GameInfo> callback)
        {
            await RequestPost("games", "/", r, callback);
        }

        // returns a game with certain id
        public async Task GetGame(int gameID, Action<Result, GameState> callback)
        {
            await RequestGet("games", "/" + gameID.ToString() + "/", callback);
        }

        /*
        string CallServer(string param = "")
        {
            WebRequest request = WebRequest.Create(host + "/" + param);
            WebResponse response = request.GetResponse();

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public void GetCurrentGame()
        {
            Console.WriteLine(CallServer("Games"));
        }
        */

        /*
        public async void PostPlayer(string url)
        {
            IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("query1", "hi"),
                new KeyValuePair<string, string>("query2", "bye"),
            };

            HttpContent q = new FormUrlEncodedContent(queries);
            
            q = new StringContent("dfd");
            using (HttpClient client = new HttpClient()) 
            {
                using (HttpResponseMessage response = await client.PostAsync(url, q))
                {
                    using (HttpContent content = response.Content)
                    {
                        string contentStr = await content.ReadAsStringAsync();
                       // HttpContentHeaders headers = content.Headers;
                        Console.WriteLine(contentStr);
                    }
                }
            }
        }
        */
    }
}
